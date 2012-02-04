using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System.Linq;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

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

  public abstract class UserViewModelBase
  {
    [DisplayName("Username:")]
    [Required(ErrorMessage = "Required")]
    public string Username { get; set; }

    [DisplayName("First")]
    [Required(ErrorMessage = "Required")]
    public string FirstName { get; set; }

    [DisplayName("Last")]
    [Required(ErrorMessage = "Required")]
    public string LastName { get; set; }

    [DisplayName("Email address:")]
    [Required(ErrorMessage = "Required")]
    [Email(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; }
  }

  public class CreateViewModel : UserViewModelBase
  {
    [DisplayName("Password:")]
    [Required(ErrorMessage = "Required")]
    public string Password { get; set; }

    [DisplayName("Confirm password:")]
    [Required(ErrorMessage = "Required")]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
  }

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
  }

  public class RoleViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }

}