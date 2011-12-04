using System;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class IndexViewModel : PagedListViewModelBase<User, UserSummaryViewModel>
  {
    public string SearchQuery { get; set; }

    public IndexViewModel(IQueryable<User> users, int page, int pageSize, Func<User, UserSummaryViewModel> converter)
      : base(users, page, pageSize, converter) {
    }
  }

}
