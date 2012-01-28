using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;

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
}