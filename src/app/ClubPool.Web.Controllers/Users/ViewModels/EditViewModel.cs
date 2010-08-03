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
  public class EditViewModel : ValidatableViewModel
  {
    public EditViewModel() {
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

    [DisplayName("Username:")]
    [NotNullNotEmpty(Message = "Required")]
    public string Username { get; set; }

    [DisplayName("First")]
    [NotNullNotEmpty(Message = "Required")]
    public string FirstName { get; set; }

    [DisplayName("Last")]
    [NotNullNotEmpty(Message = "Required")]
    public string LastName { get; set; }

    [DisplayName("Email address:")]
    [NotNullNotEmpty(Message = "Required")]
    [Email(Message = "Enter a valid email address")]
    public string Email { get; set; }

    [DisplayName("Approved")]
    public bool IsApproved { get; set; }

    [DisplayName("Locked")]
    public bool IsLocked { get; set; }

    public int[] Roles { get; set; }

    [DisplayName("Roles:")]
    public IEnumerable<RoleViewModel> AvailableRoles { get; set; }
  }

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
