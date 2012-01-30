using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClubPool.Web.Controllers.Login
{
  public class LoginViewModel
  {
    [DisplayName("Username:")]
    [Required(ErrorMessage = "Enter your username")]
    public string Username { get; set; }

    [DisplayName("Password:")]
    [Required(ErrorMessage = "Enter your password")]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }

    [DisplayName("Stay logged in")]
    public bool StayLoggedIn { get; set; }
  }

  public class LoginStatusViewModel
  {
    public bool UserIsLoggedIn { get; set; }
    public string Username { get; set; }
  }

}
