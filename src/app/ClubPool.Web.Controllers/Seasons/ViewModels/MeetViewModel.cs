using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;
using ClubPool.Framework.Extensions;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class MeetViewModel
  {
    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
    public bool IsComplete { get; set; }
    public int Week { get; set; }
    public int Id { get; set; }

    public MeetViewModel() {
    }

    public MeetViewModel(Meet meet) {
      Id = meet.Id;
      Week = meet.Week;
      IsComplete = meet.IsComplete;
      Team1Name = meet.Team1.Name;
      Team2Name = meet.Team2.Name;
    }
  }
}
