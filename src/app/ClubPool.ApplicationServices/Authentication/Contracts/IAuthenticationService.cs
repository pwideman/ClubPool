using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

using ClubPool.Core;

namespace ClubPool.ApplicationServices.Authentication.Contracts
{
  public interface IAuthenticationService
  {
    bool IsLoggedIn();
    void LogIn(string userName, bool createPersistentCookie);
    void LogOut();
    ClubPoolPrincipal GetCurrentPrincipal();
  }
}
