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
using ClubPool.Web.Controllers.Shared.ViewModels;
using ClubPool.Web.Controllers.Teams;

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
    protected static User officerUser;
    protected static ViewResultHelper<DetailsViewModel> resultHelper;
    protected static Team team;

    Establish context = () => {
      users = new List<User>();
      divisions = new List<Division>();
      teams = new List<Team>();
      matches = new List<Match>();
      meets = new List<Meet>();
      matchResults = new List<MatchResult>();
      season = DomainModelHelper.CreateTestSeason(users, divisions, teams, meets, matches, matchResults);
      adminUser = users[0];
      officerUser = users[1];
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
      team = teams[0];
    };
  }

  public class when_asked_for_the_details_view_by_an_administrator : specification_for_teams_details
  {
    static int numberOfSeasonResults;

    Establish context = () => {
      authService.MockPrincipal.User = adminUser;
      numberOfSeasonResults = meets.Where(m => m.Teams.Contains(team) && m.Matches.Where(match => match.IsComplete).Any()).Count();
    };

    Because of = () => resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));

    It should_return_the_team_name = () =>
      resultHelper.Model.Name.ShouldEqual(team.Name);

    It should_allow_the_user_to_update_the_team_name = () =>
      resultHelper.Model.CanUpdateName.ShouldBeTrue();

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

  public class when_asked_for_the_details_view_by_an_officer : specification_for_teams_details
  {
    Establish context = () => {
      authService.MockPrincipal.User = officerUser;
    };

    Because of = () => resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));

    It should_allow_the_user_to_update_the_team_name = () =>
      resultHelper.Model.CanUpdateName.ShouldBeTrue();
  }

  public class when_asked_for_the_details_view_by_a_team_member : specification_for_teams_details
  {
    Establish context = () => {
      var user = team.Players.First();
      authService.MockPrincipal.User = user;
    };

    Because of = () => resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));

    It should_allow_the_user_to_update_the_team_name = () =>
      resultHelper.Model.CanUpdateName.ShouldBeTrue();
  }

  public class when_asked_for_the_details_view_by_a_normal_user_not_on_this_team : specification_for_teams_details
  {
    Establish context = () => {
      var user = teams[1].Players.First();
      authService.MockPrincipal.User = user;
    };

    Because of = () => resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));

    It should_not_allow_the_user_to_update_the_team_name = () =>
      resultHelper.Model.CanUpdateName.ShouldBeFalse();
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_for_the_details_view_for_a_nonexistent_team : specification_for_teams_details
  {
    new static HttpNotFoundResultHelper resultHelper;

    Establish context = () => {
      authService.MockPrincipal.User = adminUser;
    };

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Details(9999));

    It should_return_http_not_found = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  public class specification_for_teams_updatename : specification_for_teams_details
  {
    protected static JsonResultHelper<AjaxUpdateResponseViewModel> resultHelper;
    protected static Team team;
    protected static UpdateNameViewModel viewModel;
    protected static string newName = "NewName";

    Establish context = () => {
      team = teams[0];
      viewModel = new UpdateNameViewModel();
      viewModel.Id = team.Id;
      viewModel.Name = newName;
    };
  }

  public class when_the_update_name_form_is_posted_by_an_admin : specification_for_teams_updatename
  {
    Establish context = () => {
      authService.MockPrincipal.User = adminUser;
    };

    Because of = () => resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));

    It should_update_the_team_name = () =>
      team.Name.ShouldEqual(newName);

    It should_return_success = () =>
      resultHelper.Data.Success.ShouldBeTrue();
  }

  public class when_the_update_name_form_is_posted_by_an_officer : specification_for_teams_updatename
  {
    Establish context = () => {
      authService.MockPrincipal.User = officerUser;
    };

    Because of = () => resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));

    It should_update_the_team_name = () =>
      team.Name.ShouldEqual(newName);

    It should_return_success = () =>
      resultHelper.Data.Success.ShouldBeTrue();
  }

  public class when_the_update_name_form_is_posted_by_a_team_member : specification_for_teams_updatename
  {
    Establish context = () => {
      authService.MockPrincipal.User = team.Players.First();
    };

    Because of = () => resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));

    It should_update_the_team_name = () =>
      team.Name.ShouldEqual(newName);

    It should_return_success = () =>
      resultHelper.Data.Success.ShouldBeTrue();
  }

  public class when_the_update_name_form_is_posted_by_a_normal_user_that_is_not_on_the_team : specification_for_teams_updatename
  {
    Establish context = () => {
      authService.MockPrincipal.User = teams.Last().Players.First();
    };

    Because of = () => resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));

    It should_not_update_the_team_name = () =>
      team.Name.ShouldNotEqual(newName);

    It should_return_failure = () =>
      resultHelper.Data.Success.ShouldBeFalse();
  }

  public class when_the_update_name_form_is_posted_with_an_empty_name : specification_for_teams_updatename
  {
    Establish context = () => {
      viewModel.Name = "";
      authService.MockPrincipal.User = adminUser;
    };

    Because of = () => resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));

    It should_not_update_the_team_name = () =>
      team.Name.ShouldNotEqual(newName);

    It should_return_failure = () =>
      resultHelper.Data.Success.ShouldBeFalse();
  }

  public class when_the_update_name_form_is_posted_with_an_invalid_id : specification_for_teams_updatename
  {
    Establish context = () => {
      viewModel.Id = 0;
      authService.MockPrincipal.User = adminUser;
    };

    Because of = () => resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));

    It should_return_failure = () =>
      resultHelper.Data.Success.ShouldBeFalse();
  }

  public class when_the_update_name_form_is_posted_for_a_nonexistent_team : specification_for_teams_updatename
  {
    new static HttpNotFoundResultHelper resultHelper;

    Establish context = () => {
      viewModel.Id = 9999;
      authService.MockPrincipal.User = adminUser;
    };

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.UpdateName(viewModel));

    It should_return_http_not_found = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  public class when_the_update_name_form_is_posted_with_a_name_in_use : specification_for_teams_updatename
  {
    Establish context = () => {
      viewModel.Name = teams[2].Name;
      authService.MockPrincipal.User = adminUser;
    };

    Because of = () => resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));

    It should_not_update_the_team_name = () =>
      team.Name.ShouldNotEqual(newName);

    It should_return_failure = () =>
      resultHelper.Data.Success.ShouldBeFalse();
  }
}
