using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class LoginStatusViewModel : ViewModelBase
  {
    public bool UserIsLoggedIn { get; set; }
    public string Username { get; set; }
  }
}
