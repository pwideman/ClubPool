﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.ViewModels
{

  public class ViewModelBase
  {
    public bool IsLoggedIn { get; set; }
    public string Username { get; set; }
  }

  public class LoginViewModel : ViewModelBase
  {
    public string Message { get; set; }
    public string ReturnUrl { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
  }
}
