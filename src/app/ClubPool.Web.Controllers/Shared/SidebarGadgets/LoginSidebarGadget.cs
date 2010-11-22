using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Shared.SidebarGadgets
{
  public class LoginSidebarGadget : SidebarGadgetBase
  {
    public static string Name = "Login";

    public LoginSidebarGadget() {
      var request = new PartialRequest();
      request.SetAction<ClubPool.Web.Controllers.Users.UsersController>(c => c.LoginGadget());
      Action = request;
    }

  }
}
