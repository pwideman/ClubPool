using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class UnapprovedViewModel
  {
    public IEnumerable<UnapprovedUser> UnapprovedUsers;
  }

  public class UnapprovedUser : EntityViewModelBase
  {
    public UnapprovedUser()
      : base() {
    }

    public UnapprovedUser(User user)
      : base(user) {
      Name = user.FullName;
      Email = user.Email;
    }

    public string Name { get; set; }
    public string Email { get; set; }
  }
}
