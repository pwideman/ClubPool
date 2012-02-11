using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Infrastructure.Configuration;

namespace ClubPool.Web.Infrastructure
{
  public class GlobalViewBagFilter : ActionFilterAttribute
  {
    private ClubPoolConfiguration config;

    public GlobalViewBagFilter(ClubPoolConfiguration config) {
      this.config = config;
    }

    public override void OnResultExecuting(ResultExecutingContext filterContext) {
      var viewResult = filterContext.Result as ViewResult;
      if (null != viewResult) {
        viewResult.ViewBag.SiteName = config.SiteName;
      }
    }
  }
}