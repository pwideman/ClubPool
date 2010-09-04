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
      CanDelete = division.CanDelete();
      HasSchedule = division.Schedule.Any();
      Teams = division.Teams.Select(t => new TeamViewModel(t)).ToList();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool CanDelete { get; set; }
    public bool HasSchedule { get; set; }
    public IEnumerable<TeamViewModel> Teams { get; set; }
  }
}
