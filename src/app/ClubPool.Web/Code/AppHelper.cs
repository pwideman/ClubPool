using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubPool.Web.Code
{
  public static class AppHelper
  {

    /// <summary>
    /// Returns an absolute reference to the Content directory
    /// </summary>
    public static string ContentRoot {
      get {

        string contentVirtualRoot = "~/Content";
        return VirtualPathUtility.ToAbsolute(contentVirtualRoot);

      }
    }

    /// <summary>
    /// Returns an absolute reference to the Images directory
    /// </summary>
    public static string ImageRoot {
      get {

        return string.Format("{0}/{1}", ContentRoot, "images");

      }
    }

    /// <summary>
    /// Returns an absolute reference to the CSS directory
    /// </summary>
    public static string CssRoot {
      get {

        return string.Format("{0}/{1}", ContentRoot, "css");

      }
    }

    public static string JsRoot {
      get {
        return string.Format("{0}/{1}", ContentRoot, "js");
      }
    }

    /// <summary>
    /// Builds an Image URL
    /// </summary>
    /// <param name="imageFile">The file name of the image</param>
    public static string ImageUrl(string imageFile) {
      string result = string.Format("{0}/{1}", ImageRoot, imageFile);
      return result;
    }

    /// <summary>
    /// Builds a CSS URL
    /// </summary>
    /// <param name="cssFile">The name of the CSS file</param>
    public static string CssUrl(string cssFile) {
      string result = string.Format("{0}/{1}", CssRoot, cssFile);
      return result;
    }

    public static string JsUrl(string jsFile) {
      return string.Format("{0}/{1}", JsRoot, jsFile);
    }

  }
}
