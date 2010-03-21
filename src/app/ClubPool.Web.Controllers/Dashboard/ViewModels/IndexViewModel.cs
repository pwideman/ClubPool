using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Dashboard.ViewModels
{
  public class IndexViewModel : BaseViewModel
  {
    public bool UserIsAdmin { get; set; }
    public IList<Core.User> NewUsersAwaitingApproval { get; set; }
  }
}
