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
    private const string JQueryUICssPath = "/jqueryui/1.8.17/themes/redmond/jquery-ui.css";
    private const string ScriptTagFormat = "<script type=\"text/javascript\" src=\"{0}\"></script>";
    private const string CssTagFormat = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />";

    private static MvcHtmlString Asset(string tagFormat, string localAsset, string remoteAsset, bool forceRelease, HtmlHelper helper) {
      string assetPath = null;
      if (!forceRelease && helper.ViewContext.HttpContext.IsDebuggingEnabled) {
        assetPath = VirtualPathUtility.ToAbsolute(LocalAssetBasePath + "/" + localAsset);
      }
      else {
        assetPath = RemoteAssetBasePath + "/" + remoteAsset;
      }
      return MvcHtmlString.Create(string.Format(tagFormat, assetPath));
    }

    private static MvcHtmlString ScriptAsset(string localScript, string remoteScript, bool forceRelease, HtmlHelper helper) {
      return Asset(ScriptTagFormat, localScript, remoteScript, forceRelease, helper);
    }

    private static MvcHtmlString CssAsset(string localCss, string remoteCss, bool forceRelease, HtmlHelper helper) {
      return Asset(CssTagFormat, localCss, remoteCss, forceRelease, helper);
    }

    public static MvcHtmlString JQuery(this HtmlHelper helper, bool forceRelease = false) {
      return ScriptAsset("jquery.min.js", JQueryPath, forceRelease, helper);
    }

    public static MvcHtmlString JQueryUIScript(this HtmlHelper helper, bool forceRelease = false) {
      return ScriptAsset("jquery-ui.min.js", JQueryUIScriptPath, forceRelease, helper);
    }

    public static MvcHtmlString JQueryUICss(this HtmlHelper helper, bool forceRelease = false) {
      return CssAsset("jquery-ui.css", JQueryUICssPath, forceRelease, helper);
    }
  }
}