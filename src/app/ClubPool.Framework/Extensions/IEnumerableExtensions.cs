using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core;

namespace ClubPool.Framework.Extensions
{
  public static class IEnumerableExtensions
  {
    public static void Each<T>(this IEnumerable<T> items, Action<T> action) {
      Check.Require(null != items, "items cannot be null");
      Check.Require(null != action, "action cannot be null");

      foreach (T item in items) {
        action(item);
      }
    }
  }
}
