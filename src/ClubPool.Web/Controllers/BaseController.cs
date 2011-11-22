using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net;

using ClubPool.Web.Controllers.Attributes;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers
{
  [ElmahRescue("DefaultError")]
  public abstract class BaseController : Controller
  {
    protected bool ValidateViewModel(object viewModel) {
      return false;
    }

    protected string BuildUrlFromExpression<T>(Expression<Action<T>> action, RouteValueDictionary values) where T : Controller {
      var rvd = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);
      foreach (var entry in RouteData.Values) {
        if (!rvd.ContainsKey(entry.Key)) {
          rvd.Add(entry.Key, entry.Value);
        }
      }
      if (null != values && values.Count > 0) {
        foreach (var value in values) {
          if (!rvd.ContainsKey(value.Key)) {
            rvd.Add(value.Key, value.Value);
          }
        }
      }

      VirtualPathData vpd = ControllerContext.RouteData.Route.GetVirtualPath(ControllerContext.RequestContext, rvd);
      return (vpd == null) ? null : vpd.VirtualPath;
    }

    protected ActionResult ErrorView(string message) {
      TempData[GlobalViewDataProperty.PageErrorMessage] = message;
      return View("Error");
    }

    protected JsonResult AjaxUpdate() {
      return AjaxUpdate(true, null);
    }

    protected JsonResult AjaxUpdate(bool success) {
      return AjaxUpdate(success, null);
    }

    protected JsonResult AjaxUpdate(bool success, string message) {
      return Json(new AjaxUpdateResponseViewModel(success, message));
    }
  }
}
