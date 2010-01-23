﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.ApplicationServices.Interfaces
{
  public interface IAuthenticationService
  {
    bool IsLoggedIn();
    void LogIn(string userName, bool createPersistentCookie);
    void LogOut();
  }
}
