using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.CurrentSeason.ViewModels
{
  public class CurrentSeasonScheduleViewModel
  {
    public IEnumerable<ScheduleDivisionViewModel> Divisions { get; set; }
    public string Name { get; set; }
    public bool HasDivisions { get; set; }
  }

  public class ScheduleDivisionViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool HasSchedule { get; set; }
    public ScheduleViewModel Schedule { get; set; }
  }
}
