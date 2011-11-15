using System;
using System.Web;
using System.Web.Security;

namespace ClubPool.Web.Services.Authentication
{
  public class FormsAuthenticationService : IAuthenticationService
  {
    public bool IsLoggedIn() {
      return HttpContext.Current.User.Identity.IsAuthenticated;
    }

    public void LogIn(string userName, bool createPersistentCookie) {
      FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
    }

    public void LogOut() {
      FormsAuthentication.SignOut();
    }

    public ClubPoolPrincipal GetCurrentPrincipal() {
      return HttpContext.Current.User as ClubPoolPrincipal;
    }
  }
}
