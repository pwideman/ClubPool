using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Matches.ViewModels
{
  public class UserHistoryViewModel : PagedListViewModelBase<UserHistoryMatchViewModel>
  {
    public bool HasMatches { get; set; }
    public string Name { get; set; }

    public UserHistoryViewModel(User user, IQueryable<UserHistoryMatchViewModel> matches, int page, int pageSize) : base(matches, page, pageSize) {
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
      var team1 = teams[0];
      var team2 = teams[1];
      Team1 = team1.Name;
      Team2 = team2.Name;
      var players = match.Players.ToArray();
      Player1 = players[0].Player.FullName;
      Player2 = players[1].Player.FullName;
      if (match.IsComplete) {
        Winner = match.Winner.FullName;
        if (!match.IsForfeit) {
          Date = match.DatePlayed;
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
