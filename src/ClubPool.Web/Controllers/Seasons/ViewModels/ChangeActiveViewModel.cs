using System;
using System.Collections.Generic;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class ChangeActiveViewModel : ViewModelBase
  {
    public string CurrentActiveSeasonName { get; set; }

    public IEnumerable<SeasonSummaryViewModel> InactiveSeasons { get; set; }
  }
}
