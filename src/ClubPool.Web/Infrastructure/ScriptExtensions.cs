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
    public static void RegisterScriptView(this HtmlHelper helper, string viewName) {
      var registrar = DependencyResolver.Current.GetService<ScriptViewRegistrar>();
      registrar.RegisterScriptView(viewName);
    }

    public static MvcHtmlString RenderRegisteredScriptViews(this HtmlHelper helper) {
      var registrar = DependencyResolver.Current.GetService<ScriptViewRegistrar>();
      if (registrar.HasRegisteredScriptViews) {
        var tagFormat = @"<script type=""text/javascript"">$(function(){{{0}}})</script>";
        var registrationFormat = @"$.scriptRegistrar.initViewScript(""{0}"");";
        var scripts = new StringBuilder();
        foreach (var viewName in registrar.GetScriptViews()) {
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