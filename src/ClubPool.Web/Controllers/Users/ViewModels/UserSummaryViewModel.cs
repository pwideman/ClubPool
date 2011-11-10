using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class UserSummaryViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; }
    public bool IsLocked { get; set; }
    public string[] Roles { get; set; }

    public UserSummaryViewModel() {
      InitMembers();
    }

    public User User {
      set {
        Id = value.Id;
        Name = value.FullName;
        Username = value.Username;
        Email = value.Email;
        IsApproved = value.IsApproved;
        IsLocked = value.IsLocked;
        if (value.Roles.Any()) {
          Roles = value.Roles.Select(r => r.Name).ToArray();
        }

      }
    }

    public UserSummaryViewModel(User user) : this() {
      User = user;
    }

    protected void InitMembers() {
      Roles = new string[0];
    }
  }
}
