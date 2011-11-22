using System.Linq;

namespace ClubPool.Web.Infrastructure
{
  public static class IQueryableExtensions
  {
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int index, int size) {
      Arg.NotNull(query, "query");
      Arg.Require(0 <= index, "index must be >= 0");
      Arg.Require(0 < size, "size must be > 0");
      return query.Skip(index * size).Take(size);
    }
  }
}
