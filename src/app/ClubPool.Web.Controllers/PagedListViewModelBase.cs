using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;
using ClubPool.Framework.Extensions;

namespace ClubPool.Web.Controllers
{
  public abstract class PagedListViewModelBase<T> : PagedListViewModelBase
  {
    public PagedListViewModelBase(IQueryable<T> source, int page, int pageSize) {
      Total = source.Count();
      TotalPages = (int)Math.Ceiling((double)Total / (double)pageSize);
      CurrentPage = Math.Min(page, TotalPages);
      var index = Math.Max(CurrentPage - 1, 0);
      Items = source.Page(index, pageSize).ToList();
      First = index * pageSize + 1;
      Last = First + Items.Count() - 1;
    }

    public IEnumerable<T> Items { get; set; }
  }

  public abstract class PagedListViewModelBase : ViewModelBase
  {
    public int CurrentPage { get; set; }
    public int First { get; set; }
    public int Last { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
  }
}
