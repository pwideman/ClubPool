using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Services.Configuration;

namespace ClubPool.Web.Infrastructure
{
  public class GlobalViewBagFilter : ActionFilterAttribute
  {
    private IConfigurationService configService;

    public GlobalViewBagFilter(IConfigurationService configService) {
      this.configService = configService;
    }

    public override void OnResultExecuting(ResultExecutingContext filterContext) {
      var viewResult = filterContext.Result as ViewResult;
      if (null != viewResult) {
        viewResult.ViewBag.SiteName = configService.GetConfig().SiteName;
      }
    }
  }
}