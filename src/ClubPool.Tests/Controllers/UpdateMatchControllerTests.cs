using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.UpdateMatch;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.UpdateMatch.when_asked_to_update_match_results
{
  public abstract class UpdateMatchControllerTest : SpecificationContext
  {
    protected UpdateMatchController controller;
    protected MockAuthenticationService authService;
    protected JsonResultHelper<UpdateMatchResponseViewModel> resultHelper;
    protected UpdateMatchViewModel viewModel;
    protected Web.Models.Match match;
    protected User player1;
    protected User player2;
    protected int player1SkillLevel;
    protected int player2SkillLevel;
    protected User loggedInUser;

    public override void EstablishContext() {
      var repository = new Mock<IRepository>();
      authService = AuthHelper.CreateMockAuthenticationService();
      controller = new UpdateMatchController(repository.Object, authService);

      loggedInUser = new User("admin", "pass", "first", "last", "email");
      loggedInUser.SetIdTo(3);
      loggedInUser.AddRole(new Role(Roles.Administrators));
      authService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      authService.MockPrincipal.MockIdentity.Name = loggedInUser.Username;


      var season = new Season("test", GameType.EightBall);
      var division = new Division("test", DateTime.Parse("1/1/2011"), season);
      season.AddDivision(division);
      var team1 = new Team("team1", division);
      division.AddTeam(team1);
      player1 = new User("player1", "test", "player1", "test", "test");
      player1.SetIdTo(1);
      team1.AddPlayer(player1);
      var team2 = new Team("team2", division);
      division.AddTeam(team2);
      player2 = new User("player2", "test", "player2", "test", "test");
      player2.SetIdTo(2);
      team2.AddPlayer(player2);
      var meet = new Meet(team1, team2, 1);
      division.Meets.Add(meet);
      match = new Web.Models.Match(meet, new MatchPlayer(player1, team1), new MatchPlayer(player2, team2));
      match.SetIdTo(1);

      repository.Setup(r => r.Get<Web.Models.Match>(match.Id)).Returns(match);
      repository.Setup(r => r.Get<User>(player1.Id)).Returns(player1);
      repository.Setup(r => r.Get<User>(player2.Id)).Returns(player2);

      var player1Results = new List<MatchResult>();
      var player2Results = new List<MatchResult>();
      for (int i = 0; i < 4; i++) {
        var tempMatch = new Web.Models.Match(meet, new MatchPlayer(player1, team1), new MatchPlayer(player2, team2));
        meet.AddMatch(tempMatch);
        tempMatch.DatePlayed = DateTime.Parse("8/1/2010").AddDays(i);
        tempMatch.IsComplete = true;
        var matchResult = new MatchResult(player1, 30, 0, 3);
        player1Results.Add(matchResult);
        tempMatch.AddResult(matchResult);
        matchResult = new MatchResult(player2, 30, 0, 3);
        player2Results.Add(matchResult);
        tempMatch.AddResult(matchResult);
      }
      repository.InitAll(new List<User> { player1, player2, loggedInUser }.AsQueryable(), 
        loggedInUser.Roles.AsQueryable(),
        new List<Season> { season }.AsQueryable());
      player1.UpdateSkillLevel(GameType.EightBall, repository.Object);
      player1SkillLevel = player1.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).First().Value;
      player2.UpdateSkillLevel(GameType.EightBall, repository.Object);
      player2SkillLevel = player2.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).First().Value;

      viewModel = new UpdateMatchViewModel() {
        Id = match.Id,
        IsForfeit = false,
        Player1Id = player1.Id,
        Player1Innings = 2,
        Player1DefensiveShots = 1,
        Player1Wins = 4,
        Player2Id = player2.Id,
        Player2Innings = 2,
        Player2DefensiveShots = 1,
        Player2Wins = 4,
        Winner = player1.Id,
        Date = "1/1/2011",
        Time = "06:00 PM"
      };
    }

    public override void When() {
      resultHelper = new JsonResultHelper<UpdateMatchResponseViewModel>(controller.Index(viewModel));
    }

  }

  [TestFixture]
  public class with_an_invalid_view_model : UpdateMatchControllerTest
  {
    public override void Given() {
      controller.ModelState.AddModelError("Id", new Exception("Test"));
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_a_validation_result_for_id() {
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Id").Count().Should().Be(1);
    }
  }

  [TestFixture]
  public class with_an_invalid_date : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Date = "abc";
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class with_an_invalid_time : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Time = "abc";
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class with_player1_defensive_shots_greater_than_innings : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Player1DefensiveShots = 21;
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class with_player2_defensive_shots_greater_than_innings : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Player2DefensiveShots = 21;
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class and_the_winner_has_less_than_two_wins : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Player1Wins = 1;
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class for_a_nonexistant_match : UpdateMatchControllerTest
  {
    private HttpNotFoundResultHelper notFoundResultHelper;

    public override void Given() {
      viewModel.Id = 10;
    }

    public override void When() {
      notFoundResultHelper = new HttpNotFoundResultHelper(controller.Index(viewModel));
    }

    [Test]
    public void it_should_return_http_not_found() {
      notFoundResultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class and_player1_is_not_a_match_player : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Player1Id = loggedInUser.Id;
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class and_player1_is_invalid : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Player1Id = 10;
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class and_the_logged_in_user_does_not_have_permission : UpdateMatchControllerTest
  {
    public override void Given() {
      loggedInUser.RemoveAllRoles();
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class and_player2_is_not_a_match_player : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Player2Id = loggedInUser.Id;
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class and_player2_is_invalid : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.Player2Id = 10;
    }

    [Test]
    public void it_should_return_success_false() {
      resultHelper.Data.Success.Should().BeFalse();
    }

    [Test]
    public void it_should_return_an_error_message() {
      resultHelper.Data.Message.Should().NotBeNullOrEmpty();
    }
  }

  [TestFixture]
  public class and_the_match_is_forfeited : UpdateMatchControllerTest
  {
    public override void Given() {
      viewModel.IsForfeit = true;
    }

    [Test]
    public void it_should_return_success_true() {
      resultHelper.Data.Success.Should().BeTrue();
    }

    [Test]
    public void it_should_set_the_match_to_complete() {
      match.IsComplete.Should().BeTrue();
    }

    [Test]
    public void it_should_set_the_match_to_forfeit() {
      match.IsForfeit.Should().BeTrue();
    }

    [Test]
    public void it_should_not_add_any_results_to_the_match() {
      match.Results.Should().BeEmpty();
    }

    [Test]
    public void it_should_set_the_match_winner() {
      match.Winner.Should().Be(player1);
    }

    [Test]
    public void it_should_not_update_player1_skill_level() {
      player1.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).First().Value.Should().Be(player1SkillLevel);
    }

    [Test]
    public void it_should_not_update_player2_skill_level() {
      player2.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).First().Value.Should().Be(player2SkillLevel);
    }
  }

  [TestFixture]
  public class with_valid_input : UpdateMatchControllerTest
  {
    [Test]
    public void it_should_return_success_true() {
      resultHelper.Data.Success.Should().BeTrue();
    }

    [Test]
    public void it_should_set_the_match_to_complete() {
      match.IsComplete.Should().BeTrue();
    }

    [Test]
    public void it_should_not_set_the_match_to_forfeit() {
      match.IsForfeit.Should().BeFalse();
    }

    [Test]
    public void it_should_set_the_match_winner() {
      match.Winner.Should().Be(player1);
    }

    [Test]
    public void it_should_add_player1_results_to_the_match() {
      match.Results.Where(r => r.Player == match.Players.First().Player).Count().Should().Be(1);
    }

    [Test]
    public void it_should_set_player1_results_innings_correctly() {
      match.Results.Where(r => r.Player == match.Players.First().Player).First().Innings.Should().Be(viewModel.Player1Innings);
    }

    [Test]
    public void it_should_set_player1_defensive_shots_correctly() {
      match.Results.Where(r => r.Player == match.Players.First().Player).First().DefensiveShots.Should().Be(viewModel.Player1DefensiveShots);
    }

    [Test]
    public void it_should_set_player1_wins_correctly() {
      match.Results.Where(r => r.Player == match.Players.First().Player).First().Wins.Should().Be(viewModel.Player1Wins);
    }

    [Test]
    public void it_should_add_player2_results_to_the_match() {
      match.Results.Where(r => r.Player == match.Players.ElementAt(1).Player).Count().Should().Be(1);
    }

    [Test]
    public void it_should_set_player2_results_innings_correctly() {
      match.Results.Where(r => r.Player == match.Players.ElementAt(1).Player).First().Innings.Should().Be(viewModel.Player2Innings);
    }

    [Test]
    public void it_should_set_player2_defensive_shots_correctly() {
      match.Results.Where(r => r.Player == match.Players.ElementAt(1).Player).First().DefensiveShots.Should().Be(viewModel.Player2DefensiveShots);
    }

    [Test]
    public void it_should_set_player2_wins_correctly() {
      match.Results.Where(r => r.Player == match.Players.ElementAt(1).Player).First().Wins.Should().Be(viewModel.Player2Wins);
    }

    // TODO: test that skill levels are updated. Will need a better mock repository to do so

    [Test]
    public void it_should_set_the_match_date() {
      match.DatePlayed.Should().Be(DateTime.Parse(viewModel.Date + " " + viewModel.Time));
    }
  }

  [TestFixture]
  public class by_a_match_player : UpdateMatchControllerTest
  {
    public override void Given() {
      authService.MockPrincipal.MockIdentity.Name = player1.Username;
    }

    [Test]
    public void it_should_return_success_true() {
      resultHelper.Data.Success.Should().BeTrue();
    }
  }
}
