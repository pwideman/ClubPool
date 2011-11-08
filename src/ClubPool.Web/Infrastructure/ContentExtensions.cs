using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Microsoft.Web.Mvc;

namespace ClubPool.Web.Infrastructure
{
  /// <summary>
  /// Custom extensions to the System.Web.Mvc.HtmlHelper class
  /// </summary>
  public static class ContentExtensions
  {
    public static MvcHtmlString ContentImage(this HtmlHelper helper, string image, string alt) {
      return helper.Image("~/content/images/" + image, alt);
    }

    public static MvcHtmlString ContentImage(this HtmlHelper helper, string image, string alt, object htmlAttributes) {
      return helper.Image("~/content/images/" + image, alt, htmlAttributes);
    }

    public static string ContentImageUrl(this UrlHelper helper, string image) {
      return helper.Content("~/content/images/" + image);
    }
  }
}
