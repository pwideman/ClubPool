using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class ScheduleWeekViewModel
  {
    public int Week { get; set; }
    public IEnumerable<MeetViewModel> Meets { get; set; }
  }
}
