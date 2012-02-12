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
    private const string JQueryUIScriptPath = "/jqueryui/1.8.17/jquery-ui.min.js";
    private const string ScriptTagFormat = "<script type=\"text/javascript\" src=\"{0}\"></script>";

    private static MvcHtmlString ScriptAsset(string localAsset, string remoteAsset, bool forceRelease, HtmlHelper helper) {
      string scriptPath = null;
      if (!forceRelease && helper.ViewContext.HttpContext.IsDebuggingEnabled) {
        scriptPath = VirtualPathUtility.ToAbsolute(LocalAssetBasePath + "/" + localAsset);
      }
      else {
        scriptPath = RemoteAssetBasePath + "/" + remoteAsset;
      }
      return MvcHtmlString.Create(string.Format(ScriptTagFormat, scriptPath));
    }

    public static MvcHtmlString JQuery(this HtmlHelper helper, bool forceRelease = false) {
      return ScriptAsset("jquery.min.js", JQueryPath, forceRelease, helper);
    }

    public static MvcHtmlString JQueryUIScript(this HtmlHelper helper, bool forceRelease = false) {
      return ScriptAsset("jquery-ui.min.js", JQueryUIScriptPath, forceRelease, helper);
    }
  }
}