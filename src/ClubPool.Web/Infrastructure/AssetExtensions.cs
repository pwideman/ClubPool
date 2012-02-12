using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubPool.Web.Infrastructure
{
  public static class AssetExtensions
  {
    private const string LocalAssetBasePath = "~/content/assets";
    private const string RemoteAssetBasePath = "http://ajax.googleapis.com/ajax/libs";
    private const string JQueryPath = "/jquery/1.7.1/jquery.min.js";
    private const string ScriptTagFormat = "<script type=\"text/javascript\" src=\"{0}\"></script>";

    public static MvcHtmlString JQuery(this HtmlHelper helper, bool forceRelease = false) {
      string scriptPath = null;
      if (!forceRelease && helper.ViewContext.HttpContext.IsDebuggingEnabled) {
        scriptPath = VirtualPathUtility.ToAbsolute(LocalAssetBasePath + "/jquery.min.js");
      }
      else {
        scriptPath = RemoteAssetBasePath + JQueryPath;
      }
      return MvcHtmlString.Create(string.Format(ScriptTagFormat, scriptPath));
    }
  }
}