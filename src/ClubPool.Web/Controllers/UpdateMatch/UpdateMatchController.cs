using System;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Models;
using ClubPool.Web.Services.Authentication;

namespace ClubPool.Web.Controllers.UpdateMatch
{
  public class UpdateMatchController : BaseController, IRouteRegistrar
  {
    protected IRepository repository;
    protected IAuthenticationService authService;

    public UpdateMatchController(IRepository repository,
      IAuthenticationService authService) {

      Arg.NotNull(repository, "repository");
      Arg.NotNull(authService, "authService");

      this.repository = repository;
      this.authService = authService;
    }

    public void RegisterRoutes(System.Web.Routing.RouteCollection routes) {
      routes.MapRoute("UpateMatch", "updatematch", new { Controller = "UpdateMatch", Action = "UpdateMatch" });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateMatch(UpdateMatchViewModel viewModel) {
      var errorViewModel = ValidateModelState(viewModel);
      if (null != errorViewModel) {
        return Json(errorViewModel);
      }

      var match = repository.Get<Match>(viewModel.Id);
      if (null == match) {
        return HttpNotFound();
      }

      var player1 = repository.Get<User>(viewModel.Player1Id);
      var player2 = repository.Get<User>(viewModel.Player2Id);
      errorViewModel = VerifyUserCanUpdateMatch(match, player1, player2);
      if (null != errorViewModel) {
        return Json(errorViewModel);
      }

      UpdateMatchFromViewModel(viewModel, match, player1, player2);
      // must save here to get new match results in DB, for calculation below
      repository.SaveChanges();

      var gameType = match.Meet.Division.Season.GameType;
      player1.UpdateSkillLevel(gameType, repository);
      player2.UpdateSkillLevel(gameType, repository);
      repository.SaveChanges();

      return Json(new UpdateMatchResponseViewModel(true));
    }

    private void UpdateMatchFromViewModel(UpdateMatchViewModel viewModel, Match match, User player1, User player2) {
      if (match.IsComplete) {
        var results = match.Results.ToList();
        foreach (var result in results) {
          repository.Delete(result);
        }
        match.Results.Clear();
      }
      match.Winner = viewModel.Winner == player1.Id ? player1 : player2;
      match.IsComplete = true;
      match.IsForfeit = viewModel.IsForfeit;

      if (!match.IsForfeit) {
        match.DatePlayed = DateTime.Parse(viewModel.Date + " " + viewModel.Time);
        var matchResult = new MatchResult(player1,
          viewModel.Player1Innings,
          viewModel.Player1DefensiveShots,
          viewModel.Player1Wins);
        match.AddResult(matchResult);

        matchResult = new MatchResult(player2,
          viewModel.Player2Innings,
          viewModel.Player2DefensiveShots,
          viewModel.Player2Wins);
        match.AddResult(matchResult);
      }
      // set meet to complete if all matches are complete
      var meet = match.Meet;
      meet.IsComplete = !meet.Matches.Where(m => !m.IsComplete).Any();
    }

    private UpdateMatchResponseViewModel VerifyUserCanUpdateMatch(Match match, User player1, User player2) {
      // authorize only admins, officers, and players involved in this meet
      var currentPrincipal = authService.GetCurrentPrincipal();
      var loggedInUser = repository.All<User>().Single(u => u.Username.Equals(currentPrincipal.Identity.Name));
      if (!match.Meet.UserCanEnterMatchResults(loggedInUser)) {
        return new UpdateMatchResponseViewModel(false, "You do not have permission to enter results for this match");
      }

      if (null == player1 || !match.Players.Where(p => p.Player == player1).Any()) {
        return new UpdateMatchResponseViewModel(false, "Player 1 is not a valid player for this match");
      }
      if (null == player2 || !match.Players.Where(p => p.Player == player2).Any()) {
        return new UpdateMatchResponseViewModel(false, "Player 2 is not a valid player for this match");
      }

      return null;
    }

    private UpdateMatchResponseViewModel ValidateModelState(UpdateMatchViewModel viewModel) {
      if (!ModelState.IsValid) {
        return new UpdateMatchResponseViewModel(false, "Validation errors", ModelState);
      }
      else {
        // we must perform some manual validation as well
        // this should maybe be moved to the view model itself?
        if (!viewModel.IsForfeit) {
          // verify that a valid date & time were entered
          DateTime tempDate;
          if (!DateTime.TryParse(viewModel.Date, out tempDate)) {
            return new UpdateMatchResponseViewModel(false, "Enter a valid date");
          }
          if (!DateTime.TryParse(viewModel.Time, out tempDate)) {
            return new UpdateMatchResponseViewModel(false, "Enter a valid time");
          }
          // verify that neither player's defensive shots are > innings
          if (viewModel.Player1DefensiveShots > viewModel.Player1Innings ||
              viewModel.Player2DefensiveShots > viewModel.Player2Innings) {
            return new UpdateMatchResponseViewModel(false, "Defensive shots cannot be greater than innings");
          }
          // verify that the winner has >= 2 wins
          int winnerWins = 0;
          if (viewModel.Winner == viewModel.Player1Id) {
            winnerWins = viewModel.Player1Wins;
          }
          else {
            winnerWins = viewModel.Player2Wins;
          }
          if (winnerWins < 2) {
            return new UpdateMatchResponseViewModel(false, "Winner must have at least 2 wins");
          }
        }
      }
      return null;
    }
  }
}
