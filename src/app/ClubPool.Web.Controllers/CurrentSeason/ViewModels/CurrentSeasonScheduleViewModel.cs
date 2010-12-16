using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.CurrentSeason.ViewModels
{
  public class CurrentSeasonScheduleViewModel
  {
    public IEnumerable<DivisionViewModel> Divisions { get; set; }
    public string Name { get; set; }
    public bool HasDivisions { get; set; }

    public CurrentSeasonScheduleViewModel(Season season) {
      Name = season.Name;
      if (season.Divisions.Count() > 0) {
        HasDivisions = true;
        var divisions = new List<DivisionViewModel>();
        foreach (var division in season.Divisions) {
          divisions.Add(new DivisionViewModel(division));
        }
        Divisions = divisions;
      }
      else {
        HasDivisions = false;
      }
    }
  }

  public class DivisionViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool HasSchedule { get; set; }
    public ScheduleViewModel Schedule { get; set; }

    public DivisionViewModel(Division division) {
      Id = division.Id;
      Name = division.Name;
      if (division.Meets.Count() > 0) {
        HasSchedule = true;
        Schedule = new ScheduleViewModel(division.Meets, division.StartingDate);
      }
      else {
        HasSchedule = false;
      }
    }
  }
}
