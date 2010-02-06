using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Microsoft.Web.Mvc.Internal;

namespace ClubPool.Web.Code
{
  /// <summary>
  /// Custom extensions to the System.Web.Mvc.HtmlHelper class
  /// </summary>
  public static class HtmlHelperExtensions
  {
    // The existing strongly typed RenderAction helper method in Microsoft.Web.Mvc is broken because it does not
    // add existing route data values in the ViewContext's RouteData to the values that it builds from
    // the action. At the very least, we need the "area" route data value which by default is "Root". This may be
    // needed only because of something in SharpArch or MvcContrib that uses areas.
    /// <summary>
    /// Render the output from another controller action into the view. This extension method differs from the
    /// existing strongly typed RenderAction helper in Microsoft.Web.Mvc by including all existing route data
    /// values in the ViewContext's RouteData to the values that it builds from the controller action.
    /// </summary>
    /// <typeparam name="TController">The controller whose action you want to render</typeparam>
    /// <param name="helper">The page's HtmlHelper</param>
    /// <param name="action">The action method that you want to render</param>
    public static void RenderActionForAreas<TController>(this HtmlHelper helper, Expression<Action<TController>> action) where TController : Controller {
      var rvd = ExpressionHelper.GetRouteValuesFromExpression(action);
      foreach (var entry in helper.ViewContext.RouteData.Values) {
        if (!rvd.ContainsKey(entry.Key)) {
          rvd.Add(entry.Key, entry.Value);
        }
      }
      helper.RenderRoute(rvd);
    }

    public static string ContentImage(this HtmlHelper helper, string image, string alt) {
      return helper.Image("~/content/images/" + image, alt);
    }

    //public static void RenderPartialRequest(this HtmlHelper html, string viewDataKey) {
    //  PartialRequest partial = html.ViewContext.ViewData.Eval(viewDataKey) as PartialRequest;
    //  if (partial != null)
    //    partial.Invoke(html.ViewContext);
    //}
  }
}
