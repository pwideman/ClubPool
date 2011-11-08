using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Machine.Specifications;

namespace ClubPool.MSpecTests
{
  // this class originally from the Who Can Help Me? app (http://whocanhelpme.codeplex.com/)
  /// <summary>
  /// Exension methods primarily for use in testing ActionResults
  /// </summary>
  public static class ActionResultExtensions
  {
    public static ViewResult IsAViewAnd(this ActionResult result) {
      result.ShouldBeOfType(typeof(ViewResult));

      return result as ViewResult;
    }

    public static PartialViewResult IsAPartialViewAnd(this ActionResult result) {
      result.ShouldBeOfType(typeof(PartialViewResult));

      return result as PartialViewResult;
    }

    public static RedirectResult IsARedirectAnd(this ActionResult result) {
      result.ShouldBeOfType(typeof(RedirectResult));

      return result as RedirectResult;
    }

    public static RedirectToRouteResult IsARedirectToARouteAnd(this ActionResult result) {
      result.ShouldBeOfType(typeof(RedirectToRouteResult));

      return result as RedirectToRouteResult;
    }

    public static string ControllerName(this RedirectToRouteResult result) {
      return result.RouteValues["Controller"].ToString();
    }

    public static string ActionName(this RedirectToRouteResult result) {
      return result.RouteValues["Action"].ToString();
    }

  }
}
