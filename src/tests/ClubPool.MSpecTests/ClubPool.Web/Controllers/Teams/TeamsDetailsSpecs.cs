using System.Collections.Generic;
using System.Linq;

using Rhino.Mocks;
using SharpArch.Testing;
using Machine.Specifications;

using ClubPool.Testing.Core;
using ClubPool.Core;
using ClubPool.Web.Controllers.Teams.ViewModels;
using ClubPool.Framework.Extensions;
using ClubPool.Core.Contracts;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Teams
{
  public abstract class specification_for_teams_details : specification_for_Teams_controller
  {
    protected static Season season;
    protected static IList<Division> divisions;
    protected static IList<Team> teams;
    protected static IList<Meet> meets;
    protected static IList<Match> matches;
    protected static IList<MatchResult> matchResults;
    protected static IList<User> users;
    protected static User adminUser;

    Establish context = () => {
      users = new List<User>();
      divisions = new List<Division>();
      teams = new List<Team>();
      matches = new List<Match>();
      meets = new List<Meet>();
      matchResults = new List<MatchResult>();
      season = DomainModelHelper.CreateTestSeason(users, divisions, teams, meets, matches, matchResults);
      adminUser = new User("admin", "test", "test", "test", "test@test.com");
      adminUser.SetIdTo(1000);
      adminUser.AddRole(new Role(Roles.Administrators));
      users.Add(adminUser);
      DomainModelHelper.SetUpTestRepository(divisionRepository, divisions);
      DomainModelHelper.SetUpTestRepository(userRepository, users);
      DomainModelHelper.SetUpTestRepository(teamRepository, teams);

      // set up skill levels
      var matchResultRepository = MockRepository.GenerateStub<IMatchResultRepository>();
      matchResultRepository.Stub(r => r.GetMatchResultsForPlayerAndGameType(null, GameType.EightBall)).IgnoreArguments().Return(null).WhenCalled(m => {
        var user = m.Arguments[0] as User;
        var gameType = (GameType)m.Arguments[1];
        m.ReturnValue = matchResults.Where(r => r.Player == user && r.Match.Meet.Division.Season.GameType == gameType).AsQueryable();
      });
      foreach (var user in users) {
        user.UpdateSkillLevel(season.GameType, matchResultRepository);
      }
    };
  }

  public class when_asked_for_the_details_view_by_an_administrator : specification_for_teams_details
  {
    static ViewResultHelper<DetailsViewModel> resultHelper;
    static Team team;
    static int numberOfSeasonResults;

    Establish context = () => {
      authService.MockPrincipal.User = adminUser;
      team = teams[0];
      numberOfSeasonResults = meets.Where(m => m.Teams.Contains(team) && m.Matches.Where(match => match.IsComplete).Any()).Count();
    };

    Because of = () => resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));

    It should_return_the_team_name = () =>
      resultHelper.Model.Name.ShouldEqual(team.Name);

    It should_return_the_correct_number_of_players = () =>
      resultHelper.Model.Players.Count().ShouldEqual(team.Players.Count());

    It should_return_the_correct_players = () =>
      resultHelper.Model.Players.Select(p => p.Name).Each(name => team.Players.Where(p => p.FullName.Equals(name)).Any().ShouldBeTrue());

    It should_return_the_correct_player_skill_levels = () =>
      resultHelper.Model.Players.Each(player => player.EightBallSkillLevel.ShouldEqual(
        team.Players.Where(p => p.FullName.Equals(player.Name)).Single()
        .SkillLevels.Where(sl => sl.GameType == team.Division.Season.GameType).First().Value));

    It should_return_has_season_results = () =>
      resultHelper.Model.HasSeasonResults.ShouldBeTrue();

    It should_return_the_correct_number_of_season_results = () =>
      resultHelper.Model.SeasonResults.Count().ShouldEqual(numberOfSeasonResults);
  }
}
