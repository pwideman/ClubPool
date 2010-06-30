using System;
using System.Collections.Generic;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class ChangeActiveViewModel : ViewModelBase
  {
    public string CurrentActiveSeasonName { get; set; }

    public IEnumerable<SeasonDto> InactiveSeasons { get; set; }
  }
}
