using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class IndexViewModel : PagedListViewModelBase<SeasonDto>
  {
    public IndexViewModel(IQueryable<SeasonDto> seasons, int page, int pageSize)
      : base(seasons, page, pageSize) {
    }
  }
}
