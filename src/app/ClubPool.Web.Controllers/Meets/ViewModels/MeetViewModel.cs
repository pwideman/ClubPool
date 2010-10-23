using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Meets.ViewModels
{
  public class MeetViewModel
  {
    public int ScheduledWeek { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TeamViewModel Team1 { get; set; }
    public TeamViewModel Team2 { get; set; }
    public IEnumerable<MatchViewModel> Matches { get; protected set; }

    public MeetViewModel() {
      Matches = new MatchViewModel[0];
    }

    public MeetViewModel(Meet meet) {
      ScheduledWeek = meet.Week + 1;
      ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7);
      Team1 = new TeamViewModel(meet.Team1);
      Team2 = new TeamViewModel(meet.Team2);
      var matches = new List<MatchViewModel>();
      foreach (var match in meet.Matches) {
        matches.Add(new MatchViewModel(match));
      }
      Matches = matches;
    }

  }

  public class TeamViewModel
  {
    public TeamViewModel() {
      Players = new List<PlayerViewModel>();
    }

    public TeamViewModel(Team team) {
      Name = team.Name;
      Players = team.Players.Select(p => new PlayerViewModel(p)).ToList();
    }

    public string Name { get; set; }
    public IEnumerable<PlayerViewModel> Players { get; set; }
  }

  public class PlayerViewModel
  {
    public PlayerViewModel() {
    }

    public PlayerViewModel(User player) {
      Name = player.FullName;
    }

    public string Name { get; set; }
  }

  public class MatchViewModel
  {
    public PlayerViewModel Player1 { get; set; }
    public PlayerViewModel Player2 { get; set; }
    public bool IsComplete { get; set; }
    public PlayerViewModel Winner { get; set; }

    public MatchViewModel() {
    }

    public MatchViewModel(Match match) {
      Player1 = new PlayerViewModel(match.Player1);
      Player2 = new PlayerViewModel(match.Player2);
      IsComplete = match.IsComplete;
      if (IsComplete) {
        Winner = new PlayerViewModel(match.Winner);
      }
    }
  }

}
