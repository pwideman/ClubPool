using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Navigation.ViewModels
{
  public class MenuViewModel
  {
    public bool DisplayAdminMenu { get; set; }
    public bool UserIsLoggedIn { get; set; }
  }
}
