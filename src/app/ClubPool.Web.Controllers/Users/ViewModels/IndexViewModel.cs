using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MvcContrib.Pagination;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class IndexViewModel : PagedListViewModelBase<UserDto>
  {
    public IndexViewModel(IQueryable<UserDto> seasons, int page, int pageSize)
      : base(seasons, page, pageSize) {
    }
  }

}
