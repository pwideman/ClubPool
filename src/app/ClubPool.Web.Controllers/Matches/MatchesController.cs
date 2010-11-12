﻿using System;
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

namespace ClubPool.Web.Controllers.Matches
{
  public class MatchesController : BaseController
  {
    protected IMatchRepository matchRepository;
    protected IUserRepository userRepository;
    protected IMatchResultRepository matchResultRepository;

    public MatchesController(IMatchRepository matchRepository, IUserRepository userRepository, IMatchResultRepository matchResultRepository) {
      Check.Require(null != matchRepository, "matchRepository cannot be null");
      Check.Require(null != userRepository, "userRepository cannot be null");
      Check.Require(null != matchResultRepository, "matchResultRepository cannot be null");

      this.matchRepository = matchRepository;
      this.userRepository = userRepository;
      this.matchResultRepository = matchResultRepository;
    }

    [HttpPost]
    [Transaction]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditMatchViewModel viewModel) {
      // TODO: Authorize only admins, officers, and a player involved in this meet
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
      return Json(new EditMatchResponseViewModel(true));
    }
  }
}
