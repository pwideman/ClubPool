using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MvcContrib.Pagination;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class IndexViewModel : ViewModelBase
  {
    public IPagination<UserDto> Users { get; set; }
    public int Page { get; set; }
  }
}
