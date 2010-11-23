using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net;

using SharpArch.Core.CommonValidator;
using xVal.ServerSide;

using ClubPool.Web.Controllers.Attributes;
using ClubPool.Framework.Validation;

namespace ClubPool.Web.Controllers
{
  [ElmahRescue("DefaultError")]
  public abstract class BaseController : Controller
  {
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

    protected bool ValidateViewModel(IValidatable viewModel) {
      try {
        viewModel.Validate();
        return true;
      }
      catch (RulesException re) {
        re.AddModelStateErrors(this.ModelState, null);
        return false;
      }
    }

    protected HttpNotFoundResult HttpNotFound(string message) {
      return new HttpNotFoundResult(message);
    }

    protected HttpNotFoundResult HttpNotFound() {
      return HttpNotFound(string.Empty);
    }

    protected ActionResult ErrorView(string message) {
      TempData[GlobalViewDataProperty.PageErrorMessage] = message;
      return View("Error");
    }
  }
}
