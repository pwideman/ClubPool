using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class RoleViewModel
  {
    public RoleViewModel() {
    }

    public RoleViewModel(Role role) {
      Id = role.Id;
      Name = role.Name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
  }
}
