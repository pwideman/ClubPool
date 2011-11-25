using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers
{
  public abstract class SidebarGadgetBase
  {
    public string Action { get; set; }
    public string Controller { get; set; }
    public object RouteValues { get; set; }
  }
}
