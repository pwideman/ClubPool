using System;
using System.Configuration;
using System.Web.Mvc;

using MvcContrib.Filters;

namespace ClubPool.Web.Controllers.Attributes
{
  public class ConfigRescueAttribute : RescueAttribute
  {
    public ConfigRescueAttribute(string view) : base(view) { }
    public ConfigRescueAttribute(string view, params Type[] exceptionTypes) : base(view, exceptionTypes) { }

    public override void OnException(ExceptionContext filterContext) {
      var useRescuesAppSetting = ConfigurationManager.AppSettings["UseRescues"];
      bool useRescues;
      if (string.IsNullOrEmpty(useRescuesAppSetting)) {
        useRescues = true;
      }
      else {
        var parsed = bool.TryParse(useRescuesAppSetting, out useRescues);
        if (!parsed) {
          useRescues = true;
        }
      }
      if (useRescues) {
        base.OnException(filterContext);
      }
    }
  }
}
