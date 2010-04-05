using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers
{
  public abstract class SidebarGadgetBase
  {
    public string Name { get; set; }
    public PartialRequest Action { get; set; }
  }
}
