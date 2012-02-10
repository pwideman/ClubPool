using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;

namespace ClubPool.Web.Infrastructure
{
  public static class ScriptExtensions
  {
    private static List<string> registrations = new List<string>();

    public static void RegisterScriptView(this HtmlHelper helper, string viewName) {
      if (!registrations.Contains(viewName)) {
        registrations.Add(viewName);
      }
    }

    public static MvcHtmlString RenderRegisteredScriptViews(this HtmlHelper helper) {
      if (registrations.Any()) {
        var tagFormat = @"<script type=""text/javascript"">$(function(){{{0}}})</script>";
        var registrationFormat = @"$.scriptRegistrar.initViewScript(""{0}"");";
        var scripts = new StringBuilder();
        foreach (var viewName in registrations) {
          scripts.Append(string.Format(registrationFormat, viewName));
        }
        return MvcHtmlString.Create(string.Format(tagFormat, scripts.ToString()));
      }
      else {
        return MvcHtmlString.Empty;
      }
    }
  }
}