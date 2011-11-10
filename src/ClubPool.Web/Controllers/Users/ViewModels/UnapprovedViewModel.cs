using System;
using System.Collections.Generic;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class UnapprovedViewModel
  {
    public IEnumerable<UnapprovedUser> UnapprovedUsers;
  }

  public class UnapprovedUser
  {
    public UnapprovedUser() {
    }

    public UnapprovedUser(User user) {
      Id = user.Id;
      Name = user.FullName;
      Email = user.Email;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
  }
}
