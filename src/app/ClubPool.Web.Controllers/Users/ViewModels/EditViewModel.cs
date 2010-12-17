using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  [Compare(Message = "Passwords do not match",
    PrimaryPropertyName = "ConfirmPassword",
    PropertyToCompare = "Password",
    Operator = xVal.Rules.ComparisonRule.Operator.Equals)]
  public class EditViewModel : UserViewModelBase
  {
    [Min(1)]
    public int Id { get; set; }

    [DisplayName("Approved")]
    public bool IsApproved { get; set; }

    [DisplayName("Locked")]
    public bool IsLocked { get; set; }

    [DisplayName("Password:")]
    public string Password { get; set; }

    [DisplayName("Confirm password:")]
    public string ConfirmPassword { get; set; }

    [Min(1)]
    public int Version { get; set; }

    public int[] Roles { get; set; }

    [DisplayName("Roles:")]
    public IEnumerable<RoleViewModel> AvailableRoles { get; set; }

    public bool ShowStatus { get; set; }
    public bool ShowRoles { get; set; }
    public bool ShowPassword { get; set; }

    public EditViewModel()
      : base() {
      InitMembers();
    }

    public EditViewModel(User user)
      : this() {
      Id = user.Id;
      FirstName = user.FirstName;
      LastName = user.LastName;
      Email = user.Email;
      IsApproved = user.IsApproved;
      IsLocked = user.IsLocked;
      Username = user.Username;
      Version = user.Version;
      Roles = user.Roles.Select(r => r.Id).ToArray();
    }

    protected void InitMembers() {
      Roles = new int[0];
      AvailableRoles = new List<RoleViewModel>();
    }

    public void LoadAvailableRoles(IRoleRepository roleRepository) {
      AvailableRoles = roleRepository.GetAll().Select(r => new RoleViewModel(r)).ToList();
    }
  }
}
