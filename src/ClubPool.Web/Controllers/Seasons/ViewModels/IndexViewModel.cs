using System;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class IndexViewModel : PagedListViewModelBase<Season, SeasonSummaryViewModel>
  {
    public IndexViewModel(IQueryable<Season> seasons, int page, int pageSize, Func<Season, SeasonSummaryViewModel> converter)
      : base(seasons, page, pageSize, converter) {
    }
  }
}
