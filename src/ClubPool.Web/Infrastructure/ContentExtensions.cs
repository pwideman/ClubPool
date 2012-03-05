using System.Web.Mvc;
using System.Web.Routing;
using System.Text;

namespace ClubPool.Web.Infrastructure
{
  /// <summary>
  /// Custom extensions to the System.Web.Mvc.HtmlHelper class
  /// </summary>
  public static class ContentExtensions
  {
    public static MvcHtmlString ContentImage(this HtmlHelper helper, string image, string alt = null, string id = null, string @class = null) {
      var tag = new StringBuilder("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\"");
      if (null != id) {
        tag.Append(" id=\"{2}\"");
      }
      if (null != @class) {
        tag.Append(" class=\"{3}\"");
      }
      tag.Append("/>");
      return new MvcHtmlString(string.Format(tag.ToString(), GetContentUrl(helper.ViewContext.RequestContext, "~/content/images/" + image), alt, id, @class));
    }

    public static string ContentImageUrl(this UrlHelper helper, string image) {
      return helper.Content("~/content/images/" + image);
    }

    public static MvcHtmlString Stylesheet(this HtmlHelper helper, string cssFile) {
      return new MvcHtmlString(string.Format("<link rel=\"Stylesheet\" type=\"text/css\" href=\"{0}\"/>", 
        GetContentUrl(helper.ViewContext.RequestContext, "~/content/css/" + cssFile)));
    }

    public static MvcHtmlString RenderSiteStylesheet(this HtmlHelper helper, bool forceRelease = false) {
      return RenderStylesheet("site", helper, forceRelease);
    }

    public static MvcHtmlString RenderScoresheetStylesheet(this HtmlHelper helper, bool forceRelease = false) {
      return RenderStylesheet("scoresheet", helper, forceRelease);
    }

    private static MvcHtmlString RenderStylesheet(string baseName, HtmlHelper helper, bool forceRelease) {
      string css = null;
      if (!forceRelease && helper.ViewContext.HttpContext.IsDebuggingEnabled) {
        css = baseName + ".less";
      }
      else {
        css = string.Format("{0}-{1}.css", baseName, DependencyResolver.Current.GetService<Configuration.ClubPoolConfiguration>().AppVersion);
      }
      return Stylesheet(helper, css);
    }

    private static string GetContentUrl(RequestContext context, string path) {
      var url = new UrlHelper(context);
      return url.Content(path);
    }
  }
}
