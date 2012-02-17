using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Views
{
  public static class ViewPageHelper
  {
    public static void RegisterScriptView(System.Web.Mvc.WebViewPage viewPage) {
      var viewName = viewPage.VirtualPath.Replace("~/Views/", "").Replace(".cshtml", "").ToLower();
      var registrar = DependencyResolver.Current.GetService<ScriptViewRegistrar>();
      registrar.RegisterScriptView(viewName);
    }
  }

  public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
  {
    public override void InitHelpers() {
      base.InitHelpers();
      ViewPageHelper.RegisterScriptView(this);
    }

    public void RegisterScript(string name) {
      var registrar = DependencyResolver.Current.GetService<ScriptViewRegistrar>();
      registrar.RegisterScriptView(name);
    }
  }

  public abstract class WebViewPage : System.Web.Mvc.WebViewPage
  {
    public override void InitHelpers() {
      base.InitHelpers();
      ViewPageHelper.RegisterScriptView(this);
    }
  }
}