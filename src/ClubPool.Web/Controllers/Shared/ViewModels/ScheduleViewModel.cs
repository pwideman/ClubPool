using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Shared.ViewModels
{
  public class ScheduleViewModel
  {
    public ScheduleViewModel() {
      Weeks = new List<ScheduleWeekViewModel>();
    }

    public ScheduleViewModel(IEnumerable<Meet> meets, DateTime startingDate, Team teamToHighlight = null) {
      Weeks = meets.GroupBy(meet => meet.Week)
        .OrderBy(g => g.Key)
        .Select(g => new ScheduleWeekViewModel() {
          Week = g.Key + 1,
          Date = startingDate.AddDays(g.Key * 7),
          Meets = g.Select(meet => new MeetViewModel(meet) {
            Highlight = (null == teamToHighlight) ? false : meet.Teams.Contains(teamToHighlight)
          })
        });
      NumberOfMeetsPerWeek = Weeks.First().Meets.Count();
    }

    public IEnumerable<ScheduleWeekViewModel> Weeks { get; set; }
    public int NumberOfMeetsPerWeek { get; set; }
  }

  public class ScheduleWeekViewModel
  {
    public int Week { get; set; }
    public DateTime Date { get; set; }
    public IEnumerable<MeetViewModel> Meets { get; set; }
  }

  public class MeetViewModel
  {
    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
    public bool IsComplete { get; set; }
    public int Week { get; set; }
    public int Id { get; set; }
    public bool Highlight { get; set; }

    public MeetViewModel() {
    }

    public MeetViewModel(Meet meet) {
      Id = meet.Id;
      Week = meet.Week;
      IsComplete = meet.IsComplete;
      var teams = meet.Teams.ToArray();
      var team1 = teams[0];
      var team2 = teams[1];
      Team1Name = team1.Name;
      Team2Name = team2.Name;
    }
  }


}
