using System;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Models;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Matches.ViewModels;

namespace ClubPool.Web.Controllers.Matches
{
  public class MatchesController : BaseController
  {
    protected IRepository repository;
    protected IAuthenticationService authService;

    public MatchesController(IRepository repository,
      IAuthenticationService authService) {

      Arg.NotNull(repository, "repository");
      Arg.NotNull(authService, "authService");

      this.repository = repository;
      this.authService = authService;
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditMatchViewModel viewModel) {
      if (!ModelState.IsValid) {
        return Json(new EditMatchResponseViewModel(false, "Validation errors", ModelState));
      }
      else {
        // we must perform some manual validation as well
        if (!viewModel.IsForfeit) {
          // verify that a valid date & time were entered
          DateTime tempDate;
          if (!DateTime.TryParse(viewModel.Date, out tempDate)) {
            return Json(new EditMatchResponseViewModel(false, "Enter a valid date"));
          }
          if (!DateTime.TryParse(viewModel.Time, out tempDate)) {
            return Json(new EditMatchResponseViewModel(false, "Enter a valid time"));
          }
          // verify that neither player's defensive shots are > innings
          if (viewModel.Player1DefensiveShots > viewModel.Player1Innings ||
              viewModel.Player2DefensiveShots > viewModel.Player2Innings) {
                return Json(new EditMatchResponseViewModel(false, "Defensive shots cannot be greater than innings"));
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
            return Json(new EditMatchResponseViewModel(false, "Winner must have at least 2 wins"));
          }
        }
      }

      var match = repository.Get<Match>(viewModel.Id);
      if (null == match) {
        return HttpNotFound();
      }

      // authorize only admins, officers, and players involved in this meet
      var currentPrincipal = authService.GetCurrentPrincipal();
      var loggedInUser = repository.All<User>().Single(u => u.Username.Equals(currentPrincipal.Identity.Name));
      if (!match.Meet.UserCanEnterMatchResults(loggedInUser)) {
        return Json(new EditMatchResponseViewModel(false, "You do not have permission to enter results for this match"));
      }

      var player1 = repository.Get<User>(viewModel.Player1Id);
      if (null == player1 || !match.Players.Where(p => p.Player == player1).Any()) {
        return Json(new EditMatchResponseViewModel(false, "Player 1 is not a valid player for this match"));
      }
      var player2 = repository.Get<User>(viewModel.Player2Id);
      if (null == player2 || !match.Players.Where(p => p.Player == player2).Any()) {
        return Json(new EditMatchResponseViewModel(false, "Player 2 is not a valid player for this match"));
      }

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
      var gameType = match.Meet.Division.Season.GameType;
      player1.UpdateSkillLevel(gameType, repository);
      player2.UpdateSkillLevel(gameType, repository);
      // set meet to complete if all matches are complete
      var meet = match.Meet;
      meet.IsComplete = !meet.Matches.Where(m => !m.IsComplete).Any();
      repository.SaveChanges();
      return Json(new EditMatchResponseViewModel(true));
    }
  }
}
