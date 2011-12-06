using System;
using System.Collections.Generic;

namespace ClubPool.Testing
{
  public static class IEnumerableExtensions
  {
    public static void Each<T>(this IEnumerable<T> items, Action<T> action) {
      foreach (T item in items) {
        action(item);
      }
    }
  }
}
