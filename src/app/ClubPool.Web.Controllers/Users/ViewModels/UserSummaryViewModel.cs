using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;
using ClubPool.Core.Queries;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class UserSummaryViewModel
  {
    public UserSummaryViewModel() {
      InitMembers();
    }

    public UserSummaryViewModel(User user) : this() {
      Id = user.Id;
      Name = user.FullName;
      Username = user.Username;
      Email = user.Email;
      IsApproved = user.IsApproved;
      IsLocked = user.IsLocked;
      CanDelete = user.CanDelete();
      if (user.Roles.Any()) {
        Roles = user.Roles.Select(RoleQueries.SelectName).ToArray();
      }
    }

    protected void InitMembers() {
      Roles = new string[0];
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; }
    public bool IsLocked { get; set; }
    public bool CanDelete { get; set; }
    public string[] Roles { get; set; }
  }
}
