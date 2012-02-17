using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Reflection;

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
        var tagFormat = "<script type=\"text/javascript\">\n$(function(){{\n{0}\n}});\n</script>";
        var registrationFormat = "$.scriptRegistrar.initViewScript(\"{0}\");\n";
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


    public static MvcHtmlString RenderSiteScripts(this HtmlHelper helper, bool forceRelease = false) {
      var appPath = DependencyResolver.Current.GetService<Configuration.ClubPoolConfiguration>().AppRootPath;
      if (!forceRelease && HttpContext.Current.IsDebuggingEnabled) {
        var scripts = new List<string>();
        AddScripts(scripts, Path.Combine(appPath, "scripts\\lib\\jquery"), "*.js");
        AddScripts(scripts, Path.Combine(appPath, "scripts\\lib"), "*.js", false);
        AddScripts(scripts, Path.Combine(appPath, "scripts"), "*.js", true);
        AddScripts(scripts, Path.Combine(appPath, "views"), "scripts.js", true, true);
        return RenderScriptTags(scripts);
      }
      else {
        // release
        return RenderScriptTags(new string[] { Path.Combine(appPath, 
          string.Format("Scripts\\site-{0}.min.js", Assembly.GetExecutingAssembly().GetName().Version.ToString())) });
      }
    }

    private static void AddScripts(List<string> scripts, string folder, string searchPattern, bool filterMinified = true, bool recursive = false) {
      var myScriptFiles = Directory.GetFiles(folder, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
      foreach (var file in myScriptFiles) {
        if (!filterMinified || !file.EndsWith(".min.js")) {
          scripts.Add(file);
        }
      }
    }

    private static MvcHtmlString RenderScriptTags(IEnumerable<string> scriptFiles) {
      var appPath = DependencyResolver.Current.GetService<Configuration.ClubPoolConfiguration>().AppRootPath;
      var tagFormat = @"<script type=""text/javascript"" src=""{0}""></script>";
      var tags = new StringBuilder();
      foreach (var file in scriptFiles) {
        var scriptUrl = VirtualPathUtility.ToAbsolute(file.Replace(appPath, "~/"));
        tags.AppendLine(string.Format(tagFormat, scriptUrl));
      }
      return MvcHtmlString.Create(tags.ToString());
    }
  }
}