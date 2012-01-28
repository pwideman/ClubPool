using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Authentication;

namespace ClubPool.Web.Controllers.Meets
{
  public class MeetsController : BaseController
  {
    protected IRepository repository;
    protected IAuthenticationService authService;

    public MeetsController(IRepository repository, IAuthenticationService authSvc) {
      Arg.NotNull(repository, "repository");
      Arg.NotNull(authSvc, "authSvc");

      this.repository = repository;
      this.authService = authSvc;
    }

    [Authorize]
    public ActionResult View(int id) {
      var meet = repository.Get<Meet>(id);
      if (null == meet) {
        return HttpNotFound();
      }

      var viewModel = CreateMeetViewModel(meet);
      var username = authService.GetCurrentPrincipal().Identity.Name;
      var loggedInUser = repository.All<User>().Single(u => u.Username.Equals(username));
      viewModel.AllowUserToEnterResults = meet.UserCanEnterMatchResults(loggedInUser);
      return View(viewModel);
    }

    private MeetViewModel CreateMeetViewModel(Meet meet) {
      var model = new MeetViewModel();
      model.Id = meet.Id;
      model.ScheduledWeek = meet.Week + 1;
      model.ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7).ToShortDateString();
      var teams = meet.Teams.ToArray();
      var team1 = teams[0];
      var team2 = teams[1];
      model.Team1Id = team1.Id;
      model.Team1Name = team1.Name;
      model.Team2Id = team2.Id;
      model.Team2Name = team2.Name;
      var matches = new List<MatchViewModel>();
      var early = true;
      foreach (var match in meet.Matches) {
        var matchViewModel = CreateMatchViewModel(match);
        if (early) {
          matchViewModel.TimeScheduled = "06:00 PM";
        }
        else {
          matchViewModel.TimeScheduled = "07:30 PM";
        }
        early = !early;
        matches.Add(matchViewModel);
      }
      model.Matches = matches;
      return model;
    }

    private MatchViewModel CreateMatchViewModel(Match match) {
      var model = new MatchViewModel();
      model.Id = match.Id;
      model.IsComplete = match.IsComplete;
      model.IsForfeit = match.IsForfeit;
      var players = match.Players.ToArray();
      model.Player1 = CreateMatchPlayerViewModel(players[0].Player, match);
      model.Player2 = CreateMatchPlayerViewModel(players[1].Player, match);
      if (match.IsComplete) {
        if (!match.IsForfeit) {
          model.DatePlayed = match.DatePlayed.Value.ToShortDateString();
          model.TimePlayed = match.DatePlayed.Value.ToShortTimeString();
          model.Status = string.Format("Played on {0} {1}", model.DatePlayed, model.TimePlayed);
        }
        else {
          model.Status = "Forfeited";
        }
      }
      else {
        model.Status = "Incomplete";
      }
      return model;
    }

    private MatchPlayerViewModel CreateMatchPlayerViewModel(User player, Match match) {
      var model = new MatchPlayerViewModel();
      model.Id = player.Id;
      model.Name = player.FullName;

      var slQuery = player.SkillLevels.Where(sl => sl.GameType == match.Meet.Division.Season.GameType);
      if (slQuery.Any()) {
        model.SkillLevel = slQuery.First().Value;
      }
      else {
        model.SkillLevel = 0;
      }

      // initialize my results, if this match is complete
      if (match.IsComplete) {
        model.Winner = match.Winner == player;
        if (!match.IsForfeit) {
          var result = match.Results.Where(r => r.Player == player).First();
          model.Innings = result.Innings.ToString();
          model.DefensiveShots = result.DefensiveShots.ToString();
          model.Wins = result.Wins.ToString();
        }
      }

      // calculate my season record & win percentage
      var completedMatches = from meet in match.Meet.Division.Meets
                             where meet.IsComplete
                             from ma in meet.Matches
                             where ma.Players.Where(p => p.Player == player).Any()
                             select ma;

      int wins = 0;
      int losses = 0;
      double winPercentage = 0;

      foreach (var completedMatch in completedMatches) {
        if (completedMatch.Winner == player) {
          wins++;
        }
        else {
          losses++;
        }
      }
      if (wins + losses > 0) {
        winPercentage = (double)wins / (double)(wins + losses);
      }
      model.Record = string.Format("{0} - {1} ({2})", wins, losses, winPercentage.ToString(".00"));
      return model;
    }

  }
}
