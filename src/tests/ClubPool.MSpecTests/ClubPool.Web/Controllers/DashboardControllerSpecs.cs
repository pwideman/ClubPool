using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Security.Principal;

using Rhino.Mocks;
using Machine.Specifications;
using SharpArch.Testing;
using SharpArch.Core.DomainModel;

using ClubPool.Web.Controllers.Dashboard;
using ClubPool.Web.Controllers.Dashboard.ViewModels;
using ClubPool.Web.Controllers.Dashboard.SidebarGadgets;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Testing.ApplicationServices.Authentication;
using ClubPool.Web.Controllers;
using ClubPool.Testing;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public class specification_for_dashboard_controller
  {
    protected static DashboardController controller;
    protected static MockAuthenticationService authenticationService;
    protected static IUserRepository userRepository;
    protected static ISeasonRepository seasonRepository;
    protected static IMatchResultRepository matchResultRepository;
    protected static IMeetRepository meetRepository;
    protected static ITeamRepository teamRepository;
    protected static Season season;
    protected static int seasonId = 1;
    protected static IList<User> users;
    protected static IList<Team> teams;
    protected static IList<MatchResult> matchResults;
    protected static IList<Match> matches;
    protected static IList<Meet> meets;

    Establish context = () => {
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      userRepository = MockRepository.GenerateStub<IUserRepository>();
      seasonRepository = MockRepository.GenerateStub<ISeasonRepository>();
      matchResultRepository = MockRepository.GenerateStub<IMatchResultRepository>();
      teamRepository = MockRepository.GenerateStub<ITeamRepository>();
      meetRepository = MockRepository.GenerateStub<IMeetRepository>();

      controller = new DashboardController(authenticationService, userRepository, seasonRepository, meetRepository, teamRepository);
      ControllerHelper.CreateMockControllerContext(controller);

      teams = new List<Team>();
      users = new List<User>();
      // set up the test season
      season = new Season("test season", GameType.EightBall);
      season.IsActive = true;
      season.SetIdTo(seasonId);
      var userId = 1;
      var division = new Division("Test Division", DateTime.Parse("1/1/2011"), season);
      division.SetIdTo(1);
      season.AddDivision(division);
      for (int j = 1; j < 13; j++) {
        var team = new Team(j.ToString(), division);
        teams.Add(team);
        division.AddTeam(team);
        for (int k = userId; k < userId + 2; k++) {
          var user = new User(k.ToString(), "test", k.ToString(), "user", "test");
          user.SetIdTo(k);
          team.AddPlayer(user);
          users.Add(user);
        }
        userId += 2;
      }
      IDivisionRepository divisionRepository = MockRepository.GenerateStub<IDivisionRepository>();
      division.CreateSchedule(divisionRepository);
      int i = 0;
      matchResults = new List<MatchResult>();
      matches = new List<Match>();
      meets = new List<Meet>();
      var meetQuery = from m in division.Meets
                      group m by m.Week into g
                      select new { Week = g.Key, Meets = g};

      foreach (var week in meetQuery) {
        var meetDate = division.StartingDate.AddDays(week.Week);
        foreach (var meet in week.Meets) {
          meets.Add(meet);
          meet.IsComplete = true;
          foreach (var match in meet.Matches) {
            match.IsComplete = true;
            var mr = new MatchResult(match.Player1, 20, 0, 3);
            match.AddResult(mr);
            matchResults.Add(mr);
            mr = new MatchResult(match.Player2, 20, 0, 2);
            match.AddResult(mr);
            matchResults.Add(mr);
            match.Winner = match.Player1;
            match.DatePlayed = meetDate;
            matches.Add(match);
          }
        }
      }
      // set up the repositories
      seasonRepository.Stub(r => r.GetAll()).Return(new List<Season>() { season }.AsQueryable());
      seasonRepository.Stub(r => r.FindOne(null)).IgnoreArguments().Return(season);
      matchResultRepository.Stub(r => r.GetMatchResultsForPlayerAndGameType(null, GameType.EightBall)).IgnoreArguments().Return(null).WhenCalled(m => {
        var user = m.Arguments[0] as User;
        var gameType = (GameType)m.Arguments[1];
        m.ReturnValue = matchResults.Where(r => r.Player == user && r.Match.Meet.Division.Season.GameType == gameType).AsQueryable();
      });
      foreach (var user in users) {
        user.UpdateSkillLevel(season.GameType, matchResultRepository);
      }
      SetUpRepository(matchResultRepository, matchResults);
      SetUpRepository(userRepository, users);
      SetUpRepository(teamRepository, teams);
      SetUpRepository(meetRepository, meets);
    };

    protected static void SetUpRepository<T>(ILinqRepository<T> repository, IList<T> list) where T : Entity {
      repository.Stub(r => r.GetAll()).Return(list.AsQueryable());
      repository.Stub(r => r.FindOne(null)).IgnoreArguments().Return(null).WhenCalled(m => {
        var criteria = m.Arguments[0] as Expression<Func<T, bool>>;
        m.ReturnValue = list.AsQueryable().Where(criteria).SingleOrDefault();
      });
      foreach (var item in list) {
        repository.Stub(r => r.Get(item.Id)).Return(item);
      }
    }
  }

  [Subject(typeof(DashboardController))]
  public class when_asked_for_the_default_view_for_nonadmin_user_with_current_season_stats : specification_for_dashboard_controller
  {
    static ViewResultHelper<IndexViewModel> resultHelper;
    static User user;
    static string username = "1";
    static int skillLevel;
    static string teamName;
    static string teammate;
    static Meet lastMeet;
    static Team team;
    static int seasonResultsCount;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.Name = username;
      user = users.Where(u => u.Username.Equals(username)).Single();
      skillLevel = user.SkillLevels.Where(sl => sl.GameType == season.GameType).First().Value;
      team = teams.Where(t => t.Players.Contains(user)).Single();
      teamName = team.Name;
      teammate = team.Players.Where(p => p != user).Single().FullName;
      lastMeet = meets.Where(m => m.Teams.Contains(team) && m.IsComplete).OrderByDescending(m => m.Week).First();
      seasonResultsCount = (from m in meets
                            where m.Teams.Contains(team) && m.IsComplete
                            from match in m.Matches
                            where match.Players.Contains(user)
                            select match).Count();
    };

    Because of = () => resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index());

    It should_indicate_that_the_user_is_not_an_administrator = () =>
      resultHelper.Model.UserIsAdmin.ShouldBeFalse();

    It should_not_include_the_alerts_gadget = () =>
      resultHelper.SidebarGadgets.Keys.ShouldNotContain(AlertsSidebarGadget.Name);

    It should_return_the_users_full_name = () =>
      resultHelper.Model.UserFullName.ShouldEqual(user.FullName);

    It should_indicate_that_there_are_current_season_stats = () =>
      resultHelper.Model.HasCurrentSeasonStats.ShouldBeTrue();

    It should_return_the_current_season_stats = () =>
      resultHelper.Model.CurrentSeasonStats.ShouldNotBeNull();

    It should_return_the_current_season_stats_skill_level = () =>
      resultHelper.Model.CurrentSeasonStats.SkillLevel.ShouldEqual(skillLevel);

    It should_return_the_current_season_stats_team_name = () =>
      resultHelper.Model.CurrentSeasonStats.TeamName.ShouldEqual(teamName);

    It should_return_the_current_season_stats_teammate = () =>
      resultHelper.Model.CurrentSeasonStats.Teammate.ShouldEqual(teammate);

    It should_return_the_current_season_stats_personal_record = () =>
      resultHelper.Model.CurrentSeasonStats.PersonalRecord.ShouldNotBeEmpty();

    It should_return_the_current_season_stats_team_record = () =>
      resultHelper.Model.CurrentSeasonStats.TeamRecord.ShouldNotBeEmpty();

    It should_return_last_meet_stats = () =>
      resultHelper.Model.HasLastMeetStats.ShouldBeTrue();

    It should_return_the_last_meet = () =>
      resultHelper.Model.LastMeetStats.OpponentTeam.ShouldEqual(lastMeet.Teams.Where(t => t != team).First().Name);

    It should_return_season_results = () =>
      resultHelper.Model.HasSeasonResults.ShouldBeTrue();

    It should_return_the_season_results = () =>
      resultHelper.Model.SeasonResults.Count().ShouldEqual(seasonResultsCount);
  }

  [Subject(typeof(DashboardController))]
  public class when_asked_for_the_default_view_for_admin_user_who_is_not_in_current_season : specification_for_dashboard_controller
  {
    static ViewResultHelper<IndexViewModel> resultHelper;

    Establish context = () => {
      var principal = authenticationService.MockPrincipal;
      principal.Roles = new string[] { Roles.Administrators };
      principal.MockIdentity.Name = "admin";
      // set up an admin user that is not in a team
      var adminUser = new User("admin", "test", "admin", "user", "test");
      adminUser.SetIdTo(1000);
      adminUser.AddRole(new Role(Roles.Administrators));
      users.Add(adminUser);
    };

    Because of = () => resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index());

    It should_indicate_that_the_user_is_an_administrator = () =>
      resultHelper.Model.UserIsAdmin.ShouldBeTrue();

    It should_include_the_alerts_gadget = () =>
      resultHelper.SidebarGadgets.Keys.ShouldContain(AlertsSidebarGadget.Name);

    It should_not_return_current_season_stats = () =>
      resultHelper.Model.HasCurrentSeasonStats.ShouldBeFalse();

    It should_not_return_last_meet_stats = () =>
      resultHelper.Model.HasLastMeetStats.ShouldBeFalse();
  }

  //[Subject(typeof(DashboardController))]
  //public class when_asked_for_the_default_view_for_admin_user_and_there_are_unapproved_users : specification_for_dashboard_controller
  //{
  //  static ViewResultHelper<IndexViewModel> resultHelper;
  //  static IList<User> users;

  //  Establish context = () => {
  //    var principal = authenticationService.MockPrincipal;
  //    principal.MockIdentity.IsAuthenticated = true;
  //    principal.Roles = new string[] { Roles.Administrators };
  //    users = new List<User>() {
  //      new User("user1", "user1", "user", "one", "test@test.com") { IsApproved = false },
  //      new User("user2", "user2", "user", "two", "two@two.com") { IsApproved = false }
  //    };
  //    userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
  //  };

  //  Because of = () => resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index());

  //  // TODO: Verify that the unapproved alert is added
  //}
}
