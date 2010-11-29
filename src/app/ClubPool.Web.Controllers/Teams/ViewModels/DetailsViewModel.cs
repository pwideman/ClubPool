using System.Collections.Generic;
using System.Linq;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class DetailsViewModel
  {
    public string Name { get; set; }
    public IEnumerable<DetailsPlayerViewModel> Players { get; set; }
    public string Record { get; set; }
    public string Rank { get; set; }
    public IEnumerable<DetailsMeetViewModel> SeasonResults { get; set; }
    public bool HasSeasonResults { get; set; }

    public DetailsViewModel(Team team) {
      Name = team.Name;
      Record = GetRecord(team);
      var players = new List<DetailsPlayerViewModel>();
      foreach(var player in team.Players) {
        players.Add(new DetailsPlayerViewModel(player));
      }
      Players = players;
      Rank = CalculateRank(team);
      var teamMeets = team.Division.Meets.Where(m => m.Teams.Contains(team) && m.Matches.Where(match => match.IsComplete).Any())
                                         .OrderByDescending(m => m.Week);
      if (teamMeets.Any()) {
        HasSeasonResults = true;
        var seasonResults = new List<DetailsMeetViewModel>();
        foreach (var meet in teamMeets) {
          seasonResults.Add(new DetailsMeetViewModel(meet, team));
        }
        SeasonResults = seasonResults;
      }
      else {
        HasSeasonResults = false;
      }
    }

    protected string CalculateRank(Team team) {
      var division = team.Division;
      var q = division.Teams.OrderByDescending(t => t.GetWinPercentage()).Select((o,i) => new { Rank = i, Team = o }).ToArray();
      var rank = q.Where(o => o.Team == team).Select(o => o.Rank).Single();
      var tied = false;
      if (rank > 0) {
        var myWinPct = team.GetWinPercentage();
        while (rank > 0 && q[rank-1].Team.GetWinPercentage() == myWinPct) {
          rank--;
        }
        tied = (q[rank + 1].Team.GetWinPercentage() == myWinPct);
      }
      return (tied ? "T" : "") + (rank+1).ToString();

    }

    protected string GetRecord(Team team) {
      var winsAndLosses = team.GetWinsAndLosses();
      var wins = winsAndLosses[0];
      var losses = winsAndLosses[1];
      var total = wins + losses;
      double winPct = 0;
      if (total > 0) {
        winPct = (double)wins / (double)total;
      }
      return string.Format("{0} - {1} ({2:.00})", wins, losses, winPct);
    }
  }

  public class DetailsPlayerViewModel
  {
    public string Name { get; set; }
    public int EightBallSkillLevel { get; set; }

    public DetailsPlayerViewModel(User player) {
      Name = player.FullName;
      var skillLevel = player.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).FirstOrDefault();
      if (null != skillLevel) {
        EightBallSkillLevel = skillLevel.Value;
      }
    }
  }

  public class DetailsMeetViewModel
  {
    public string Opponent { get; set; }
    public IEnumerable<DetailsMatchViewModel> Matches { get; set; }

    public DetailsMeetViewModel(Meet meet, Team team) {
      Opponent = meet.Teams.Where(t => t != team).Single().Name;
      var matches = new List<DetailsMatchViewModel>();
      foreach (var match in meet.Matches.Where(m => m.IsComplete)) {
        matches.Add(new DetailsMatchViewModel(match, team));
      }
      Matches = matches;
    }
  }

  public class DetailsMatchViewModel
  {
    public string TeamPlayerName { get; set; }
    public int TeamPlayerWins { get; set; }
    public string OpponentPlayerName { get; set; }
    public int OpponentPlayerWins { get; set; }
    public bool Win { get; set; }

    public DetailsMatchViewModel(Match match, Team team) {
      var teamPlayer = match.Players.Where(p => team.Players.Contains(p)).Single();
      TeamPlayerName = teamPlayer.FullName;
      var oppPlayer = match.Players.Where(p => p != teamPlayer).Single();
      OpponentPlayerName = oppPlayer.FullName;
      Win = match.Winner == teamPlayer;
      if (!match.IsForfeit) {
        var teamResult = match.Results.Where(r => r.Player == teamPlayer).Single();
        TeamPlayerWins = teamResult.Wins;
        var oppResult = match.Results.Where(r => r != teamResult).Single();
        OpponentPlayerWins = oppResult.Wins;
      }
    }
  }
}

