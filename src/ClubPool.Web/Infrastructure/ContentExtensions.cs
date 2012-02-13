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

    private static string GetContentUrl(RequestContext context, string path) {
      var url = new UrlHelper(context);
      return url.Content(path);
    }

    public static string AssetPath(this UrlHelper helper, string asset) {
      var assetManager = DependencyResolver.Current.GetService<AssetManager>();
      return assetManager.GetAssetPath(asset);
    }

    public static string LocalAssetPath(this UrlHelper helper, string asset) {
      var assetManager = DependencyResolver.Current.GetService<AssetManager>();
      return assetManager.GetLocalAssetPath(asset);
    }

  }
}
