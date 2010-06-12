using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class LoginViewModel : ValidatableViewModel
  {
    [DisplayName("Username:")]
    [NotNullNotEmpty(Message="Enter your username")]
    public string Username { get; set; }

    [DisplayName("Password:")]
    [NotNullNotEmpty(Message="Enter your password")]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }

    [DisplayName("Stay logged in")]
    public bool StayLoggedIn { get; set; }
  }
}
