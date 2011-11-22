using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
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
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string Version { get; set; }

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
      Version = user.EncodedVersion;
      Roles = user.Roles.Select(r => r.Id).ToArray();
    }

    protected void InitMembers() {
      Roles = new int[0];
      AvailableRoles = new List<RoleViewModel>();
    }

    public void LoadAvailableRoles(IRepository repository) {
      AvailableRoles = repository.All<Role>().Select(r => new RoleViewModel { Role = r }).ToList();
    }
  }
}
