using System;
using System.Collections.Generic;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Users
{
  public class IndexViewModel : PagedListViewModelBase<User, UserSummaryViewModel>
  {
    public string SearchQuery { get; set; }
  }

  public class UserSummaryViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; }
    public bool IsLocked { get; set; }
    public string[] Roles { get; set; }
  }

}