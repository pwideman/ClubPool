using System;
using System.Configuration;
using System.Web.Mvc;

using MvcContrib.Filters;

using ClubPool.Framework.Configuration;

namespace ClubPool.Web.Controllers.Attributes
{
  public class ConfigRescueAttribute : RescueAttribute
  {
    public ConfigRescueAttribute(string view) : base(view) { }
    public ConfigRescueAttribute(string view, params Type[] exceptionTypes) : base(view, exceptionTypes) { }

    public override void OnException(ExceptionContext filterContext) {
      bool useRescues = ClubPoolConfigurationSection.GetConfig().UseRescues;
      if (useRescues) {
        base.OnException(filterContext);
      }
    }
  }
}
