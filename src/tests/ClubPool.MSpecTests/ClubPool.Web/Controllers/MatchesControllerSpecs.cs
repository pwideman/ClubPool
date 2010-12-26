using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Net;

using Rhino.Mocks;
using Machine.Specifications;
using SharpArch.Testing;

using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Matches;
using ClubPool.Web.Controllers.Matches.ViewModels;
using ClubPool.Framework.NHibernate;
using ClubPool.Framework.Extensions;
using ClubPool.Testing;
using ClubPool.Testing.Core;
using ClubPool.Testing.ApplicationServices.Authentication;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_Matches_controller
  {
    protected static MatchesController controller;
    protected static IMatchRepository matchRepository;
    protected static IUserRepository userRepository;
    protected static IMatchResultRepository matchResultRepository;
    protected static MockAuthenticationService authService;
    protected static JsonResultHelper<EditMatchResponseViewModel> resultHelper;
    protected static EditMatchViewModel viewModel;
    protected static Match match;
    protected static User player1;
    protected static User player2;
    protected static int matchId = 1;
    protected static int player1Id = 1;
    protected static int player2Id = 2;
    protected static int player1SkillLevel;
    protected static int player2SkillLevel;
    protected static string username = "adminuser";
    protected static User loggedInUser;

    Establish context = () => {
      matchRepository = MockRepository.GenerateStub<IMatchRepository>();
      userRepository = MockRepository.GenerateStub<IUserRepository>();
      matchResultRepository = MockRepository.GenerateStub<IMatchResultRepository>();
      authService = AuthHelper.CreateMockAuthenticationService();
      controller = new MatchesController(matchRepository, userRepository, matchResultRepository, authService);

      authService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      authService.MockPrincipal.MockIdentity.Name = username;
      loggedInUser = new User(username, "pass", "first", "last", "email");
      loggedInUser.AddRole(new Role(Roles.Administrators));
      userRepository.Stub(r => r.FindOne(null)).IgnoreArguments().WhenCalled(m => m.ReturnValue = loggedInUser);

      ControllerHelper.CreateMockControllerContext(controller);
      ServiceLocatorHelper.AddValidator();

      viewModel = new EditMatchViewModel() {
        Id = matchId,
        IsForfeit = false,
        Player1Id = player1Id,
        Player1Innings = 20,
        Player1DefensiveShots = 1,
        Player1Wins = 4,
        Player2Id = player2Id,
        Player2Innings = 20,
        Player2DefensiveShots = 1,
        Player2Wins = 4,
        Winner = player1Id,
        Date = "1/1/2011",
        Time = "06:00 PM"
      };

      var season = new Season("test", GameType.EightBall);
      var division = new Division("test", DateTime.Parse("1/1/2011"), season);
      season.AddDivision(division);
      var team1 = new Team("team1", division);
      division.AddTeam(team1);
      player1 = new User("player1", "test", "player1", "test", "test");
      player1.SetIdTo(player1Id);
      team1.AddPlayer(player1);
      var team2 = new Team("team2", division);
      division.AddTeam(team2);
      player2 = new User("player2", "test", "player2", "test", "test");
      player2.SetIdTo(player2Id);
      team2.AddPlayer(player2);
      var meet = new Meet(team1, team2, 1);
      match = new Match(meet, player1, player2);
      match.SetIdTo(matchId);

      matchRepository.Stub(r => r.Get(matchId)).Return(match);
      userRepository.Stub(r => r.Get(player1Id)).Return(player1);
      userRepository.Stub(r => r.Get(player2Id)).Return(player2);

      var player1Results = new List<MatchResult>();
      var player2Results = new List<MatchResult>();
      for(int i = 0;i<4;i++) {
        var tempMatch = new Match(meet, player1, player2);
        tempMatch.DatePlayed = DateTime.Parse("8/1/2010").AddDays(i);
        tempMatch.IsComplete = true;
        var matchResult = new MatchResult(player1, 30, 0, 3);
        player1Results.Add(matchResult);
        tempMatch.AddResult(matchResult);
        matchResult = new MatchResult(player2, 30, 0, 3);
        player2Results.Add(matchResult);
        tempMatch.AddResult(matchResult);
      }
      matchResultRepository.Stub(r => r.GetMatchResultsForPlayerAndGameType(player1, GameType.EightBall)).Return(player1Results.AsQueryable());
      matchResultRepository.Stub(r => r.GetMatchResultsForPlayerAndGameType(player2, GameType.EightBall)).Return(player2Results.AsQueryable());
      player1.UpdateSkillLevel(GameType.EightBall, matchResultRepository);
      player1SkillLevel = player1.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).First().Value;
      player2.UpdateSkillLevel(GameType.EightBall, matchResultRepository);
      player2SkillLevel = player2.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).First().Value;
    };
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_match_results_with_an_invalid_view_model : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel = new EditMatchViewModel() {
        Player1Innings = -1,
        Player1DefensiveShots = -1,
        Player1Wins = 10,
        Player2Innings = -1,
        Player2DefensiveShots = -1,
        Player2Wins = 10
      };
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_a_validation_result_for_id = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Id").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_winner = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Winner").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player1_id = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player1Id").Count().ShouldEqual(1);
    
    It should_return_a_validation_result_for_player2_id = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player2Id").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player1_wins = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player1Wins").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player2_wins = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player2Wins").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player1_innings = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player1Innings").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player2_innings = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player2Innings").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player1_defensive_shots = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player1DefensiveShots").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player2_defensive_shots = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player2DefensiveShots").Count().ShouldEqual(1);
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_and_date_is_invalid : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel.Date = "abc";
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_an_error_message = () =>
      resultHelper.Data.Message.ShouldEqual("Enter a valid date");
  }


  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_and_time_is_invalid : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel.Time = "abc";
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_an_error_message = () =>
      resultHelper.Data.Message.ShouldEqual("Enter a valid time");
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_and_player1_defensive_shots_are_greater_than_innings : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel.Player1DefensiveShots = 21;
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_an_error_message = () =>
      resultHelper.Data.Message.ShouldEqual("Defensive shots cannot be greater than innings");
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_and_player2_defensive_shots_are_greater_than_innings : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel.Player2DefensiveShots = 21;
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_an_error_message = () =>
      resultHelper.Data.Message.ShouldEqual("Defensive shots cannot be greater than innings");
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_and_the_winner_has_less_than_two_wins : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel.Player1Wins = 1;
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_an_error_message = () =>
      resultHelper.Data.Message.ShouldEqual("Winner must have at least 2 wins");
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_nonexistent_match : specification_for_Matches_controller
  {
    new static HttpNotFoundResultHelper resultHelper;
    Establish context = () => {
      viewModel.Id = 10;
    };

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Edit(viewModel));

    It should_return_http_not_found = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_when_player1_is_not_a_match_player : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel.Player1Id = 10;
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_an_error_message = () =>
      resultHelper.Data.Message.ShouldEqual("Player 1 is not a valid player for this match");
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_when_player2_is_not_a_match_player : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel.Player2Id = 10;
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_an_error_message = () =>
      resultHelper.Data.Message.ShouldEqual("Player 2 is not a valid player for this match");
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_forfeited_match : specification_for_Matches_controller
  {
    Establish context = () => {
      viewModel.IsForfeit = true;
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_true = () =>
      resultHelper.Data.Success.ShouldBeTrue();

    It should_set_the_match_to_complete = () =>
      match.IsComplete.ShouldBeTrue();

    It should_set_the_match_to_forfeit = () =>
      match.IsForfeit.ShouldBeTrue();

    It should_not_add_any_results_to_the_match = () =>
      match.Results.ShouldBeEmpty();

    It should_set_the_match_winner = () =>
      match.Winner.ShouldEqual(player1);

    It should_not_update_player1_skill_level = () =>
      player1.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).First().Value.ShouldEqual(player1SkillLevel);

    It should_not_update_player2_skill_level = () =>
      player2.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).First().Value.ShouldEqual(player2SkillLevel);
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match : specification_for_Matches_controller
  {
    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_true = () =>
      resultHelper.Data.Success.ShouldBeTrue();

    It should_set_the_match_to_complete = () =>
      match.IsComplete.ShouldBeTrue();

    It should_not_set_the_match_to_forfeit = () =>
      match.IsForfeit.ShouldBeFalse();

    It should_add_player1_results_to_the_match = () =>
      match.Results.Where(r => r.Player == match.Players.First()).Count().ShouldEqual(1);

    It should_set_player1_results_innings_correctly = () =>
      match.Results.Where(r => r.Player == match.Players.First()).First().Innings.ShouldEqual(viewModel.Player1Innings);

    It should_set_player1_defensive_shots_correctly = () =>
      match.Results.Where(r => r.Player == match.Players.First()).First().DefensiveShots.ShouldEqual(viewModel.Player1DefensiveShots);

    It should_set_player1_wins_correctly = () =>
      match.Results.Where(r => r.Player == match.Players.First()).First().Wins.ShouldEqual(viewModel.Player1Wins);

    It should_add_player2_results_to_the_match = () =>
      match.Results.Where(r => r.Player == match.Players.ElementAt(1)).Count().ShouldEqual(1);

    It should_set_player2_results_innings_correctly = () =>
      match.Results.Where(r => r.Player == match.Players.ElementAt(1)).First().Innings.ShouldEqual(viewModel.Player2Innings);

    It should_set_player2_defensive_shots_correctly = () =>
      match.Results.Where(r => r.Player == match.Players.ElementAt(1)).First().DefensiveShots.ShouldEqual(viewModel.Player2DefensiveShots);

    It should_set_player2_wins_correctly = () =>
      match.Results.Where(r => r.Player == match.Players.ElementAt(1)).First().Wins.ShouldEqual(viewModel.Player2Wins);

    It should_set_the_match_winner = () =>
      match.Winner.ShouldEqual(player1);

    It should_set_the_match_date = () =>
      match.DatePlayed.ShouldEqual(DateTime.Parse(viewModel.Date + " " + viewModel.Time));

    // TODO: Maybe figure out some way to test skill level updates, can't do that now because
    // we've stubbed matchResultRepository.GetResultsForPlayerAndGameType, so the new results
    // aren't included
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_and_the_logged_in_user_does_not_have_permission : specification_for_Matches_controller
  {
    Establish context = () =>
      loggedInUser.RemoveAllRoles();

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_an_error_message = () =>
      resultHelper.Data.Message.ShouldEqual("You do not have permission to enter results for this match");
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_a_match_by_a_match_player : specification_for_Matches_controller
  {
    Establish context = () =>
      loggedInUser = player1;

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_true = () =>
      resultHelper.Data.Success.ShouldBeTrue();
  }
}
