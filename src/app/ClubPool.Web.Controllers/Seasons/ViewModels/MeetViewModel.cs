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
    public string[] TeamNames { get; set; }
    public bool IsComplete { get; set; }
    public int Week { get; set; }

    public MeetViewModel() {
    }

    public MeetViewModel(Meet meet) {
      Week = meet.Week;
      IsComplete = meet.IsComplete;
      TeamNames = meet.Teams.Select(team => team.Name).ToArray();
    }
  }
}
