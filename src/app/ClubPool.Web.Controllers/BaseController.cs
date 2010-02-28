using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using ClubPool.Web.Controllers.Attributes;

namespace ClubPool.Web.Controllers
{
  [ElmahRescue("DefaultError")]
  public abstract class BaseController : Controller
  {
  }
}
