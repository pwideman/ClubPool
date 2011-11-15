using System;

namespace ClubPool.Web.Services.Authentication
{
  public interface IAuthenticationService
  {
    bool IsLoggedIn();
    void LogIn(string userName, bool createPersistentCookie);
    void LogOut();
    ClubPoolPrincipal GetCurrentPrincipal();
  }
}
