using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class LoginStatusViewModel
  {
    public bool UserIsLoggedIn { get; set; }
    public string Username { get; set; }
  }
}
