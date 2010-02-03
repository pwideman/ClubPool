using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers
{
  public class BaseViewModel
  {
    public LoginStatusViewModel LoginStatusViewModel { get; set; }
    public MenuViewModel MenuViewModel { get; set; }

    public void Init(BaseViewModel viewModel) {
      this.LoginStatusViewModel = viewModel.LoginStatusViewModel;
      this.MenuViewModel = viewModel.MenuViewModel;
    }
  }

  public class LoginStatusViewModel
  {
    public bool IsLoggedIn { get; set; }
    public string Username { get; set; }
  }

  public class MenuViewModel
  {
    public bool DisplayAdminMenu { get; set; }
  }
}
