using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Seasons
{
  public class IndexViewModel : PagedListViewModelBase<Season, SeasonSummaryViewModel>
  {
  }

  public class SeasonSummaryViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool CanDelete { get; set; }
  }

  public class ChangeActiveViewModel
  {
    public string CurrentActiveSeasonName { get; set; }
    public IEnumerable<SeasonSummaryViewModel> InactiveSeasons { get; set; }
  }

  public class EditSeasonViewModel : CreateSeasonViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    [Required]
    public string Version { get; set; }
  }

  public class CreateSeasonViewModel
  {
    [DisplayName("Name:")]
    [Required(ErrorMessage = "Required")]
    public string Name { get; set; }
  }

  public class SeasonViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<DivisionViewModel> Divisions { get; set; }
  }

  public class DivisionViewModel
  {
    public int Id { get; set; }
    public DateTime StartingDate { get; set; }
    public string Name { get; set; }
    public bool CanDelete { get; set; }
    public bool HasSchedule { get; set; }
    public bool HasEnoughTeamsForSchedule { get; set; }
    public bool HasCompletedMatches { get; set; }
    public IEnumerable<TeamViewModel> Teams { get; set; }
    public ScheduleViewModel Schedule { get; set; }
  }

  public class TeamViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public bool CanDelete { get; set; }
    public IEnumerable<PlayerViewModel> Players { get; set; }
  }

  public class PlayerViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }

}