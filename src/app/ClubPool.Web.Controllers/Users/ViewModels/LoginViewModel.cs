using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class LoginViewModel : FormViewModelBase
  {
    [DisplayName("Username:")]
    public string Username { get; set; }

    [DisplayName("Password:")]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }

    [DisplayName("Stay logged in")]
    public bool StayLoggedIn { get; set; }
  }
}
