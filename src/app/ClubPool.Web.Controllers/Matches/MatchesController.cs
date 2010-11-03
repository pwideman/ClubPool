using System;
using System.Linq;
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

    public MatchesController(IMatchRepository matchRepository, IUserRepository userRepository) {
      Check.Require(null != matchRepository, "matchRepository cannot be null");
      Check.Require(null != userRepository, "userRepository cannot be null");

      this.matchRepository = matchRepository;
      this.userRepository = userRepository;
    }

    [HttpPost]
    [Transaction]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditMatchViewModel viewModel) {
      var match = matchRepository.Get(viewModel.Id);
      if (match.IsComplete) {
        match.RemoveAllResults();
      }
      var player1 = userRepository.Get(viewModel.Player1.Id);
      var matchResult = new MatchResult(player1, 
        viewModel.Player1.Innings, 
        viewModel.Player1.DefensiveShots, 
        viewModel.Player1.Wins);
      match.AddResult(matchResult);

      var player2 = userRepository.Get(viewModel.Player2.Id);
      matchResult = new MatchResult(player2,
        viewModel.Player2.Innings,
        viewModel.Player2.DefensiveShots,
        viewModel.Player2.Wins);
      match.AddResult(matchResult);

      match.Winner = viewModel.Winner == player1.Id ? player1 : player2;
      match.IsComplete = true;
      return new EmptyResult();
    }
  }
}
