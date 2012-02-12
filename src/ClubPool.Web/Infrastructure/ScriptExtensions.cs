using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using System.IO;
using System.Diagnostics;

using SquishIt.Framework.JavaScript;

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

    private static IEnumerable<string> viewScripts;
    private static bool viewsDirectoryScanned = false;
    private static object syncLock = new object();

    public static JavaScriptBundle AddViewScripts(this JavaScriptBundle bundle, bool forceRelease = false) {
      if (!forceRelease && HttpContext.Current.IsDebuggingEnabled) {
        lock (syncLock) {
          viewScripts = ScanViewsDirectory();
          viewsDirectoryScanned = true;
        }
      }
      else if (!viewsDirectoryScanned) {
        lock (syncLock) {
          if (!viewsDirectoryScanned) {
            viewScripts = ScanViewsDirectory();
            viewsDirectoryScanned = true;
          }
        }
      }
      bundle.Add(viewScripts.ToArray());
      return bundle;
    }

    private static IEnumerable<string> ScanViewsDirectory() {
      var appPath = DependencyResolver.Current.GetService<Configuration.ClubPoolConfiguration>().AppRootPath;
      var scriptFiles = new List<string>();
      var pathToScan = Path.Combine(appPath, "Views");
      Trace.TraceInformation("Scanning path for view scripts: " + pathToScan);
      ScanDirectoryForScripts(pathToScan, scriptFiles);
      return scriptFiles.Select(s => s.Replace(appPath, "~/"));
    }

    private static void ScanDirectoryForScripts(string directory, List<string> scriptFiles) {
      var myScriptFiles = Directory.GetFiles(directory, "scripts.js");
      foreach (var file in myScriptFiles) {
        scriptFiles.Add(file);
      }
      foreach (var subdirectory in Directory.GetDirectories(directory)) {
        ScanDirectoryForScripts(subdirectory, scriptFiles);
      }
    }
  }
}