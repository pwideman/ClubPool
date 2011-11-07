using System;
using System.Configuration;
using System.Web.Mvc;

using MvcContrib.Filters;
using Microsoft.Practices.ServiceLocation;

using ClubPool.Web.Services.Configuration;

namespace ClubPool.Web.Controllers.Attributes
{
  public class ConfigRescueAttribute : RescueAttribute
  {
    public ConfigRescueAttribute(string view) : base(view) { }
    public ConfigRescueAttribute(string view, params Type[] exceptionTypes) : base(view, exceptionTypes) { }

    public override void OnException(ExceptionContext filterContext) {
      // I hate using ServiceLocator for this, but not much choice here
      var configService = ServiceLocator.Current.GetInstance<IConfigurationService>();
      bool useRescues = configService.GetConfig().UseRescues;
      if (useRescues) {
        base.OnException(filterContext);
      }
    }
  }
}
