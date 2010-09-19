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
      Teams = new string[0];
    }

    public MeetViewModel(Meet meet) {
      ScheduledWeek = meet.Week + 1;
      ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7);
      Teams = meet.Teams.Select(t => t.Name).ToArray();
    }

    public int ScheduledWeek { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string[] Teams { get; set; }
  }
}
