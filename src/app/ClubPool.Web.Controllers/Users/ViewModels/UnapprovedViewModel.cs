using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class UnapprovedViewModel : FormViewModelBase
  {
    public IEnumerable<UnapprovedUser> UnapprovedUsers;
  }

  public class UnapprovedUser
  {
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
  }
}
