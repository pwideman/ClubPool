using System;

namespace ClubPool.Web.Controllers.Dashboard.SidebarGadgets
{
  public class AlertsSidebarGadget : SidebarGadgetBase
  {
    public static string Name = "Alerts";

    public AlertsSidebarGadget() {
      Action = "AlertsGadget";
      Controller = "Dashboard";
    }
  }
}
