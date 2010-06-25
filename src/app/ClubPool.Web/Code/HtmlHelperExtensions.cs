using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Microsoft.Web.Mvc;

namespace ClubPool.Web.Code
{
  /// <summary>
  /// Custom extensions to the System.Web.Mvc.HtmlHelper class
  /// </summary>
  public static class HtmlHelperExtensions
  {
    public static MvcHtmlString ContentImage(this HtmlHelper helper, string image, string alt) {
      return helper.Image("~/content/images/" + image, alt);
    }
  }
}
