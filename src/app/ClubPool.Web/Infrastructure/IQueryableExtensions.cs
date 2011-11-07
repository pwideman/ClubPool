using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core;

namespace ClubPool.Web.Infrastructure
{
  public static class IQueryableExtensions
  {
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int index, int size) {
      Check.Require(null != query, "query cannot be null");
      Check.Require(0 <= index, "index must be >= 0");
      Check.Require(0 < size, "size must be > 0");
      return query.Skip(index * size).Take(size);
    }
  }
}
