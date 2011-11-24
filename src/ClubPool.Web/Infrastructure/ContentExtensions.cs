using System.Web.Mvc;
using System.Web.Routing;

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

    public static MvcHtmlString Stylesheet(this HtmlHelper helper, string cssFile) {
      return new MvcHtmlString(string.Format("<link rel=\"Stylesheet\" type=\"text/css\" href=\"{0}\"/>", 
        GetContentUrl(helper.ViewContext.RequestContext, "~/content/css/" + cssFile)));
    }

    public static MvcHtmlString ScriptInclude(this HtmlHelper helper, string jsFile) {
      return new MvcHtmlString(string.Format("<script type=\"text/javascript\" src=\"{0}\" ></script>\n", 
        GetContentUrl(helper.ViewContext.RequestContext, "~/scripts/" + jsFile)));
    }

    private static string GetContentUrl(RequestContext context, string path) {
      var url = new UrlHelper(context);
      return url.Content(path);
    }
  }
}
