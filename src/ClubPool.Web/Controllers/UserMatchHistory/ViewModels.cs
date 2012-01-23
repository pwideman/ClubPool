using System;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.UserMatchHistory
{
  public class UserHistoryViewModel : PagedListViewModelBase<Match, UserHistoryMatchViewModel>
  {
    public bool HasMatches { get; set; }
    public string Name { get; set; }
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
  }
}
