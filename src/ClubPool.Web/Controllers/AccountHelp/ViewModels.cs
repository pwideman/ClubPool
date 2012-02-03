using System;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

namespace ClubPool.Web.Controllers.AccountHelp
{
  public class RecoverUsernameViewModel
  {
    [Email]
    [Required]
    public string Email { get; set; }
  }

  public class ResetPasswordViewModel
  {
    public string Username { get; set; }
    public string Email { get; set; }
  }

}