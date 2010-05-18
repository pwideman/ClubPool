using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Dashboard.SidebarGadgets
{
  public class AlertsSidebarGadget : SidebarGadgetBase
  {
    public AlertsSidebarGadget() {
      Name = "Alerts";
      var request = new PartialRequest();
      request.SetAction<ClubPool.Web.Controllers.Dashboard.DashboardController>(c => c.AlertsGadget());
      Action = request;
    }
  }
}
