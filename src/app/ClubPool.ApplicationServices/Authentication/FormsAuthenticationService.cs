﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.Core;

namespace ClubPool.Web.Security
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

    public IPrincipal GetCurrentPrincipal() {
      return HttpContext.Current.User;
    }
  }
}
