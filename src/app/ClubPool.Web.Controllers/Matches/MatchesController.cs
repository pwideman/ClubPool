using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using SharpArch.Core;
using SharpArch.Web.NHibernate;

using ClubPool.Web.Controllers.Matches.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Framework;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.ApplicationServices.Authentication.Contracts;

namespace ClubPool.Web.Controllers.Matches
{
  public class MatchesController : BaseController
  {
    protected IMatchRepository matchRepository;
    protected IUserRepository userRepository;
    protected IMatchResultRepository matchResultRepository;
    protected IAuthenticationService authService;

    public MatchesController(IMatchRepository matchRepository,
      IUserRepository userRepository,
      IMatchResultRepository matchResultRepository,
      IAuthenticationService authService) {

      Check.Require(null != matchRepository, "matchRepository cannot be null");
      Check.Require(null != userRepository, "userRepository cannot be null");
      Check.Require(null != matchResultRepository, "matchResultRepository cannot be null");
      Check.Require(null != authService, "authService cannot be null");

      this.matchRepository = matchRepository;
      this.userRepository = userRepository;
      this.matchResultRepository = matchResultRepository;
      this.authService = authService;
    }

    [HttpGet]
    [Transaction]
    [Authorize]
    public ActionResult UserHistory(int id, int? page) {
      var user = userRepository.Get(id);
      if (null == user) {
        return HttpNotFound();
      }

      var userMatches = from match in matchRepository.GetAll()
                        where (match.Players.Contains(user)) && match.IsComplete
                        orderby match.DatePlayed descending
                        select new UserHistoryMatchViewModel(match);
      var viewModel = new UserHistoryViewModel(user, userMatches, page.GetValueOrDefault(1), 15);
      return View(viewModel);
    }

    [HttpPost]
    [Transaction]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditMatchViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return Json(new EditMatchResponseViewModel(false, "Validation errors", viewModel.ValidationResults()));
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

      var match = matchRepository.Get(viewModel.Id);
      if (null == match) {
        return HttpNotFound();
      }

      // authorize only admins, officers, and players involved in this meet
      var currentPrincipal = authService.GetCurrentPrincipal();
      var loggedInUser = userRepository.FindOne(u => u.Username.Equals(currentPrincipal.Identity.Name));
      if (!match.Meet.UserCanEnterMatchResults(loggedInUser)) {
        return Json(new EditMatchResponseViewModel(false, "You do not have permission to enter results for this match"));
      }

      var player1 = userRepository.Get(viewModel.Player1Id);
      if (null == player1 || !match.Players.Contains(player1)) {
        return Json(new EditMatchResponseViewModel(false, "Player 1 is not a valid player for this match"));
      }
      var player2 = userRepository.Get(viewModel.Player2Id);
      if (null == player2 || !match.Players.Contains(player2)) {
        return Json(new EditMatchResponseViewModel(false, "Player 2 is not a valid player for this match"));
      }

      if (match.IsComplete) {
        match.RemoveAllResults();
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
      player1.UpdateSkillLevel(gameType, matchResultRepository);
      player2.UpdateSkillLevel(gameType, matchResultRepository);
      // set meet to complete if all matches are complete
      var meet = match.Meet;
      meet.IsComplete = !meet.Matches.Where(m => !m.IsComplete).Any();
      return Json(new EditMatchResponseViewModel(true));
    }
  }
}
