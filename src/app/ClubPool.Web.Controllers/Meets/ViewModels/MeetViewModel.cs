using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Meets.ViewModels
{
  public class MeetViewModel
  {
    public MeetViewModel() {
    }

    public MeetViewModel(Meet meet) {
      ScheduledWeek = meet.Week + 1;
      ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7);
      var teams = meet.Teams.Select(t => new TeamViewModel(t)).ToArray();
      Team1 = teams[0];
      Team2 = teams[1];
    }

    public int ScheduledWeek { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TeamViewModel Team1 { get; set; }
    public TeamViewModel Team2 { get; set; }
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

}
