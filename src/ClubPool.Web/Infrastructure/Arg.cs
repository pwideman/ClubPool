using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubPool.Web.Infrastructure
{
  public static class Arg
  {
    public static void NotNull(string arg, string name) {
      if (string.IsNullOrWhiteSpace(arg))
        throw new ArgumentNullException(name);
    }

    public static void NotNull(object arg, string name) {
      if (null == arg) {
        throw new ArgumentNullException(name);
      }
    }

    public static void Require(bool assertion, string message = null) {
      if (!assertion) {
        if (!string.IsNullOrWhiteSpace(message))
          throw new ArgumentException(message);
        else
          throw new ArgumentException();
      }
    }
  }
}