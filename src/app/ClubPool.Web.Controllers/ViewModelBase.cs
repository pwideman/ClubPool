using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers
{
  public abstract class ViewModelBase
  {
  }

  public abstract class PagedListViewModelBase : ViewModelBase
  {
    public int CurrentPage { get; set; }
    public int First { get; set; }
    public int Last { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
  }

  //public class FormViewModelBase : ViewModelBase
  //{
  //  public string ErrorMessage { get; set; }
  //}
}
