using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;

using MvcContrib.Filters;
using Elmah;

namespace ClubPool.Web.Controllers.Attributes
{
  public class ElmahRescueAttribute : ConfigRescueAttribute
  {
    public ElmahRescueAttribute(string view) : base(view) { }
    public ElmahRescueAttribute(string view, params Type[] exceptionTypes) : base(view, exceptionTypes) { }

    public override void OnException(ExceptionContext filterContext) {
      ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);
      base.OnException(filterContext);
    }
  }
}
