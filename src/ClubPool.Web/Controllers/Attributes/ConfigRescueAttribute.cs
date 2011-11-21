using System;
using System.Configuration;
using System.Web.Mvc;

using MvcContrib.Filters;

using ClubPool.Web.Services.Configuration;

namespace ClubPool.Web.Controllers.Attributes
{
  public class ConfigRescueAttribute : RescueAttribute
  {
    public ConfigRescueAttribute(string view) : base(view) { }
    public ConfigRescueAttribute(string view, params Type[] exceptionTypes) : base(view, exceptionTypes) { }

    public override void OnException(ExceptionContext filterContext) {
      // TODO: Use proper filter dependency resolution here
      var configService = DependencyResolver.Current.GetService<IConfigurationService>();
      bool useRescues = configService.GetConfig().UseRescues;
      if (useRescues) {
        base.OnException(filterContext);
      }
    }
  }
}
