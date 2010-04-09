using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Dashboard.ViewModels
{
  public class IndexViewModel : ViewModelBase
  {
    public bool UserIsAdmin { get; set; }
    public IEnumerable<Core.User> NewUsersAwaitingApproval { get; set; }
  }
}
