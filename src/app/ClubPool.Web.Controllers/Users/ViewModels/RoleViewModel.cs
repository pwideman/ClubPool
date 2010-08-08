using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class RoleViewModel : EntityViewModelBase
  {
    public RoleViewModel() : base() {
    }

    public RoleViewModel(Role role) : base(role) {
      Name = role.Name;
    }

    public string Name { get; set; }
  }
}
