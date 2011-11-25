
namespace ClubPool.Web.Controllers.Shared.SidebarGadgets
{
  public class LoginSidebarGadget : SidebarGadgetBase
  {
    public static string Name = "Login";

    public LoginSidebarGadget() {
      Action = "LoginGadget";
      Controller = "Users";
    }

  }
}
