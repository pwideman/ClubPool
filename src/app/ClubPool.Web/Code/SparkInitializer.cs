using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Spark;

namespace ClubPool.Web.Code
{
  // we must initialize spark in code because adding the Microsoft.Web.Mvc namespace 
  // causes the intellisense support to fail for some reason. I put this in a class
  // so it can be referenced from Global.asax.cs and the precompile post build step
  public static class SparkInitializer
  {
    public static ISparkSettings GetSettings() {
      var settings = new SparkSettings()
        .SetPageBaseType("ClubPool.Web.Views.SparkViewBase")
        .SetAutomaticEncoding(true)
        .SetDebug(true)
        .AddAssembly(typeof(System.Web.Mvc.AcceptVerbsAttribute).Assembly)
        .AddAssembly(typeof(Microsoft.Web.Mvc.AcceptAjaxAttribute).Assembly)
        .AddAssembly(typeof(MvcContrib.DefaultConvertible).Assembly)
        .AddAssembly(typeof(MvcContrib.FluentHtml.ModelStateDictionaryExtensions).Assembly)
        .AddAssembly(typeof(xVal.ActiveRuleProviders).Assembly)
        .AddAssembly(typeof(System.Data.Linq.Binary).Assembly) // this is needed because Microsoft.Web.Mvc has a ref to it
        .AddAssembly(typeof(System.Web.Routing.HttpMethodConstraint).Assembly)
        .AddNamespace("Microsoft.Web.Mvc")
        .AddNamespace("MvcContrib")
        .AddNamespace("MvcContrib.FluentHtml")
        .AddNamespace("ClubPool.Web.Code")
        .AddNamespace("ClubPool.Web.Controllers")
        .AddNamespace("System")
        .AddNamespace("System.Collections.Generic")
        .AddNamespace("System.Linq")
        .AddNamespace("System.Web.Mvc")
        .AddNamespace("System.Web.Mvc.Html")
        .AddNamespace("xVal.Html");
      return settings;
    }
  }
}
