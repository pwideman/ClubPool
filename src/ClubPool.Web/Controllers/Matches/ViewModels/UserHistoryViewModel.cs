using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Matches.ViewModels
{
  public class UserHistoryViewModel : PagedListViewModelBase<Match, UserHistoryMatchViewModel>
  {
    public bool HasMatches { get; set; }
    public string Name { get; set; }

    public UserHistoryViewModel(User user, IQueryable<Match> matches, int page, int pageSize, Func<Match, UserHistoryMatchViewModel> converter)
      : base(matches, page, pageSize, converter) {
      Name = user.FullName;
      if (null == matches || matches.Count() == 0) {
        HasMatches = false;
      }
      else {
        HasMatches = true;
      }
    }
  }

  public class UserHistoryMatchViewModel
  {
    public string Season { get; set; }
    public string Team1 { get; set; }
    public string Team2 { get; set; }
    public string Player1 { get; set; }
    public string Player2 { get; set; }
    public string Winner { get; set; }
    public int Player1Innings { get; set; }
    public int Player2Innings { get; set; }
    public int Player1DefensiveShots { get; set; }
    public int Player2DefensiveShots { get; set; }
    public int Player1Wins { get; set; }
    public int Player2Wins { get; set; }
    public DateTime Date { get; set; }

    public UserHistoryMatchViewModel(Match match) {
      Season = match.Meet.Division.Season.Name;
      var teams = match.Meet.Teams.ToArray();
      Team1 = teams[0].Name;
      if (teams.Count() > 1) {
        Team2 = teams[1].Name;
      }
      var players = match.Players.ToArray();
      Player1 = players[0].Player.FullName;
      Player2 = players[1].Player.FullName;
      if (match.IsComplete) {
        Winner = match.Winner.FullName;
        if (!match.IsForfeit) {
          Date = match.DatePlayed.Value;
          var results = match.Results.Where(r => r.Player == players[0].Player).Single();
          Player1Innings = results.Innings;
          Player1DefensiveShots = results.DefensiveShots;
          Player1Wins = results.Wins;
          results = match.Results.Where(r => r.Player == players[1].Player).Single();
          Player2Innings = results.Innings;
          Player2DefensiveShots = results.DefensiveShots;
          Player2Wins = results.Wins;
        }
      }
    }
  }
}
