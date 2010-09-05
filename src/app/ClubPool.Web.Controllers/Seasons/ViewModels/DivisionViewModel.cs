using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

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
      if (division.Schedule.Any()) {
        Schedule = new ScheduleViewModel(division.Schedule, division.StartingDate);
        HasSchedule = true;
      }
      Teams = division.Teams.Select(t => new TeamViewModel(t)).ToList();
    }

    public int Id { get; set; }
    public DateTime StartingDate { get; set; }
    public string Name { get; set; }
    public bool CanDelete { get; set; }
    public bool HasSchedule { get; set; }
    public IEnumerable<TeamViewModel> Teams { get; set; }
    public ScheduleViewModel Schedule { get; set; }
  }

  public class ScheduleViewModel
  {
    public ScheduleViewModel() {
      Weeks = new List<ScheduleWeekViewModel>();
    }

    public ScheduleViewModel(IEnumerable<Meet> meets, DateTime startingDate) {
      Weeks = meets.GroupBy(meet => meet.Week)
        .Select(g => new ScheduleWeekViewModel() {
          Week = g.Key + 1,
          Date = startingDate.AddDays(g.Key * 7),
          Meets = g.Select(meet => new MeetViewModel(meet))
        });
      NumberOfMeetsPerWeek = Weeks.First().Meets.Count();
    }

    public IEnumerable<ScheduleWeekViewModel> Weeks { get; set; }
    public int NumberOfMeetsPerWeek { get; set; }
  }
}
