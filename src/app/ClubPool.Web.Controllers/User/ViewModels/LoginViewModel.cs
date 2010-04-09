using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.User.ViewModels
{
  public class LoginViewModel : FormViewModelBase
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
    public bool StayLoggedIn { get; set; }
  }
}
