using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubPool.Web.Infrastructure
{
  public class ScriptViewRegistrar
  {
    private List<string> registrations = new List<string>();

    public void RegisterScriptView(string viewName) {
      if (!registrations.Contains(viewName)) {
        registrations.Add(viewName);
      }
    }

    public IEnumerable<string> GetScriptViews() {
      return registrations;
    }

    public bool HasRegisteredScriptViews {
      get {
        return registrations.Any();
      }
    }
  }
}