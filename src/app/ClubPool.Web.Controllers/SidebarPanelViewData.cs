using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers
{
  public class SidebarCollection : List<SidebarPanelViewData>
  {
  }

  public class SidebarPanelViewData
  {
    public string Name { get; set; }
    public PartialRequest Action { get; set; }
  }
}
