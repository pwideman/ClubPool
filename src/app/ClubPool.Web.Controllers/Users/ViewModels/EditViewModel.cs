using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class EditViewModel : UserViewModelBase
  {
    public EditViewModel() : base() {
      InitMembers();
    }

    protected void InitMembers() {
      Roles = new int[0];
      AvailableRoles = new List<RoleViewModel>();
    }

    public void LoadAvailableRoles(ILinqRepository<Role> roleRepository) {
      AvailableRoles = roleRepository.GetAll().Select(r => new RoleViewModel(r)).ToList();
    }

    [Min(1)]
    public int Id { get; set; }

    [DisplayName("Approved")]
    public bool IsApproved { get; set; }

    [DisplayName("Locked")]
    public bool IsLocked { get; set; }

    public int[] Roles { get; set; }

    [DisplayName("Roles:")]
    public IEnumerable<RoleViewModel> AvailableRoles { get; set; }
  }

}
