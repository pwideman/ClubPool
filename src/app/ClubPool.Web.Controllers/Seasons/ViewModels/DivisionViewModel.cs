using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class DivisionViewModel
  {
    public DivisionViewModel() {
      Teams = new List<TeamViewModel>();
    }

    public DivisionViewModel(Division division) {
      Id = division.Id;
      Name = division.Name;
      StartingDate = division.StartingDate;
      CanDelete = division.CanDelete();
      if (division.Meets.Any()) {
        Schedule = new ScheduleViewModel(division.Meets, division.StartingDate);
        HasSchedule = true;
      }
      Teams = division.Teams.Select(t => new TeamViewModel(t)).ToList();
      if (Teams.Count() > 1) {
        HasEnoughTeamsForSchedule = true;
      }
      else {
        HasEnoughTeamsForSchedule = false;
      }
    }

    public int Id { get; set; }
    public DateTime StartingDate { get; set; }
    public string Name { get; set; }
    public bool CanDelete { get; set; }
    public bool HasSchedule { get; set; }
    public bool HasEnoughTeamsForSchedule { get; set; }
    public IEnumerable<TeamViewModel> Teams { get; set; }
    public ScheduleViewModel Schedule { get; set; }
  }
}
