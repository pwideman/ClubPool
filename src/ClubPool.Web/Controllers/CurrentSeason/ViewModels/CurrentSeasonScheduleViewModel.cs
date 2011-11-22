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

    public CurrentSeasonScheduleViewModel(Season season, User user) {
      Name = season.Name;
      if (season.Divisions.Count() > 0) {
        HasDivisions = true;
        var divisions = new List<ScheduleDivisionViewModel>();
        foreach (var division in season.Divisions) {
          divisions.Add(new ScheduleDivisionViewModel(division, user));
        }
        Divisions = divisions;
      }
      else {
        HasDivisions = false;
      }
    }
  }

  public class ScheduleDivisionViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool HasSchedule { get; set; }
    public ScheduleViewModel Schedule { get; set; }

    public ScheduleDivisionViewModel(Division division, User user) {
      Id = division.Id;
      Name = division.Name;
      if (division.Meets.Count() > 0) {
        HasSchedule = true;
        var team = division.Teams.Where(t => t.Players.Contains(user)).SingleOrDefault();
        Schedule = new ScheduleViewModel(division.Meets, division.StartingDate, team);
      }
      else {
        HasSchedule = false;
      }
    }
  }
}
