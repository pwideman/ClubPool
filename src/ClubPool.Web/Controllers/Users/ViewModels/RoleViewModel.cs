using System;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class RoleViewModel
  {
    public RoleViewModel() {
    }

    public Role Role {
      set {
        Id = value.Id;
        Name = value.Name;
      }
    }

    public RoleViewModel(Role role) {
      Role = role;
    }

    public int Id { get; set; }
    public string Name { get; set; }
  }
}
