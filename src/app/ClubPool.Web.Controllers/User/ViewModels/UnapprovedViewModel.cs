using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.User.ViewModels
{
  public class UnapprovedViewModel : FormViewModelBase
  {
    public IList<Core.User> UnapprovedUsers;
  }
}
