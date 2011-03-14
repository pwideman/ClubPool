using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.CurrentSeason.ViewModels
{
  public class CurrentSeasonStandingsViewModel
  {
    public string Name { get; set; }
    public IEnumerable<StandingsDivisionViewModel> Divisions { get; set; }
    public bool HasDivisions { get; set; }
    public IEnumerable<StandingsPlayerViewModel> AllPlayers { get; set; }

    public CurrentSeasonStandingsViewModel(Season season, User userToHighlight) {
      Name = season.Name;
      if (season.Divisions.Any()) {
        HasDivisions = true;
        var divisions = new List<StandingsDivisionViewModel>();
        foreach (var division in season.Divisions) {
          divisions.Add(new StandingsDivisionViewModel(division, userToHighlight));
        }
        Divisions = divisions;
        var players = new List<StandingsPlayerViewModel>();
        foreach (var division in Divisions) {
          players.AddRange(division.Players);
        }
        AllPlayers = players.OrderByDescending(p => p.WinPercentage).ThenByDescending(p => p.Wins).ThenByDescending(p => p.SkillLevel);
      }
      else {
        HasDivisions = false;
      }
    }
  }

  public class StandingsDivisionViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<StandingsTeamViewModel> Teams { get; set; }
    public IEnumerable<StandingsPlayerViewModel> Players { get; set; }
    public bool HasTeams { get; set; }
    public bool HasPlayers { get; set; }

    public StandingsDivisionViewModel(Division division, User userToHighlight) {
      Id = division.Id;
      Name = division.Name;
      HasTeams = false;
      HasPlayers = false;
      if (division.Teams.Any()) {
        HasTeams = true;
        var players = new List<StandingsPlayerViewModel>();
        var teams = new List<StandingsTeamViewModel>();
        foreach (var team in division.Teams) {
          var teamvm = new StandingsTeamViewModel(team, userToHighlight);
          if (null != teamvm.Player1) {
            players.Add(teamvm.Player1);
          }
          if (null != teamvm.Player2) {
            players.Add(teamvm.Player2);
          }
          teams.Add(teamvm);
        }
        Teams = RankList(teams);
        if (players.Any()) {
          Players = RankList(players);
          HasPlayers = true;
        }
      }
    }

    private List<T> RankList<T>(List<T> list) where T:StandingsViewModelBase {
      var newlist = list.OrderByDescending(t => t.WinPercentage).ThenByDescending(t => t.Wins).ToList();
      var count = newlist.Count;
      var currentRank = 1;
      string rank = "1";
      double prevWP = -1;
      for (int i = 0; i < count; i++) {
        var item = newlist[i];
        if (i > 0) {
          if (item.WinPercentage == prevWP) {
            rank = "T" + currentRank.ToString();
            newlist[i - 1].Rank = rank;
          }
          else {
            currentRank = i + 1;
            rank = currentRank.ToString();
          }
        }
        item.Rank = rank;
        prevWP = item.WinPercentage;
      }
      return newlist;
    }
  }

  public class StandingsTeamViewModel : StandingsViewModelBase
  {
    public StandingsPlayerViewModel Player1 { get; set; }
    public StandingsPlayerViewModel Player2 { get; set; }

    public StandingsTeamViewModel(Team team, User userToHighlight) {
      Id = team.Id;
      Name = team.Name;
      if (null != userToHighlight) {
        Highlight = team.Players.Contains(userToHighlight);
      }
      var winsAndLosses = team.GetWinsAndLosses();
      Wins = winsAndLosses[0];
      Losses = winsAndLosses[1];
      WinPercentage = (Wins + Losses > 0) ? ((double)Wins / (double)(Wins + Losses)) : 0;
      if (team.Players.Any()) {
        var players = team.Players.ToArray();
        if (players.Length > 0) {
          Player1 = new StandingsPlayerViewModel(players[0], team, userToHighlight);
          if (players.Length > 1) {
            Player2 = new StandingsPlayerViewModel(players[1], team, userToHighlight);
          }
        }
      }
    }
  }

  public class StandingsPlayerViewModel : StandingsViewModelBase
  {
    public int SkillLevel { get; set; }

    public StandingsPlayerViewModel(User player, Team team, User userToHighlight) {
      Id = player.Id;
      Name = player.FullName;
      if (null != userToHighlight) {
        Highlight = player == userToHighlight;
      }
      var gameType = team.Division.Season.GameType;
      var slQuery = player.SkillLevels.Where(sl => sl.GameType == gameType);
      if (slQuery.Any()) {
        SkillLevel = slQuery.Single().Value;
      }
      else {
        SkillLevel = 0;
      }
      var wl = team.GetWinsAndLossesForPlayer(player);
      Wins = wl[0];
      Losses = wl[1];
      WinPercentage = (Wins + Losses > 0) ? ((double)Wins / (double)(Wins + Losses)) : 0;
    }
  }

  public abstract class StandingsViewModelBase
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinPercentage { get; set; }
    public string Rank { get; set; }
    public bool Highlight { get; set; }
  }
}
