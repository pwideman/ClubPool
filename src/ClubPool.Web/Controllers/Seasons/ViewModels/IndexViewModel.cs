using System;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class IndexViewModel : PagedListViewModelBase<SeasonSummaryViewModel>
  {
    public IndexViewModel(IQueryable<SeasonSummaryViewModel> seasons, int page, int pageSize)
      : base(seasons, page, pageSize) {
    }
  }
}
