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
using ClubPool.Testing.Core;

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
    protected static IList<Division> divisions;
    protected static User adminUser;

    Establish context = () => {
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      userRepository = MockRepository.GenerateStub<IUserRepository>();
      seasonRepository = MockRepository.GenerateStub<ISeasonRepository>();
      matchResultRepository = MockRepository.GenerateStub<IMatchResultRepository>();
      teamRepository = MockRepository.GenerateStub<ITeamRepository>();
      meetRepository = MockRepository.GenerateStub<IMeetRepository>();

      controller = new DashboardController(authenticationService,
        userRepository,
        seasonRepository,
        meetRepository,
        teamRepository,
        matchResultRepository);
      ControllerHelper.CreateMockControllerContext(controller);

      teams = new List<Team>();
      users = new List<User>();
      matchResults = new List<MatchResult>();
      matches = new List<Match>();
      meets = new List<Meet>();
      divisions = new List<Division>();
      season = DomainModelHelper.CreateTestSeason(users, divisions, teams, meets, matches, matchResults);
      // set up the repositories
      DomainModelHelper.SetUpTestRepository(seasonRepository, new List<Season>() { season });
      DomainModelHelper.SetUpTestRepository(matchResultRepository, matchResults);
      DomainModelHelper.SetUpTestRepository(userRepository, users);
      DomainModelHelper.SetUpTestRepository(teamRepository, teams);
      DomainModelHelper.SetUpTestRepository(meetRepository, meets);
      matchResultRepository.Stub(r => r.GetMatchResultsForPlayerAndGameType(null, GameType.EightBall)).IgnoreArguments().Return(null).WhenCalled(m => {
        var user = m.Arguments[0] as User;
        var gameType = (GameType)m.Arguments[1];
        m.ReturnValue = matchResults.Where(r => r.Player == user && r.Match.Meet.Division.Season.GameType == gameType).AsQueryable();
      });
      foreach (var user in users) {
        user.UpdateSkillLevel(season.GameType, matchResultRepository);
      }
      adminUser = users[0];
    };
  }

  [Subject(typeof(DashboardController))]
  public class when_asked_for_the_default_view_for_nonadmin_user_with_current_season_stats : specification_for_dashboard_controller
  {
    static ViewResultHelper<IndexViewModel> resultHelper;
    static User user;
    static string username;
    static int skillLevel;
    static User teammate;
    static Meet lastMeet;
    static Team team;
    static int seasonResultsCount;

    Establish context = () => {
      user = users.Skip(2).First();
      username = user.Username;
      authenticationService.MockPrincipal.User = user;
      skillLevel = user.SkillLevels.Where(sl => sl.GameType == season.GameType).First().Value;
      team = teams.Where(t => t.Players.Contains(user)).Single();
      teammate = team.Players.Where(p => p != user).Single();
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
      resultHelper.Model.CurrentSeasonStats.TeamName.ShouldEqual(team.Name);

    It should_return_the_current_season_stats_team_id = () =>
      resultHelper.Model.CurrentSeasonStats.TeamId.ShouldEqual(team.Id);

    It should_return_the_current_season_stats_teammate_name = () =>
      resultHelper.Model.CurrentSeasonStats.TeammateName.ShouldEqual(teammate.FullName);

    It should_return_the_current_season_stats_teammate_id = () =>
      resultHelper.Model.CurrentSeasonStats.TeammateId.ShouldEqual(teammate.Id);

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
      authenticationService.MockPrincipal.User = adminUser;
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
