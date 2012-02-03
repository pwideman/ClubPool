using System;
using System.Collections.Generic;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.UnapprovedUsers
{
  public class UnapprovedUsersViewModel
  {
    public IEnumerable<UnapprovedUser> UnapprovedUsers;
  }

  public class UnapprovedUser
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
  }
}