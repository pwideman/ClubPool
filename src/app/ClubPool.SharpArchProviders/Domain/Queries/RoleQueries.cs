using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ClubPool.SharpArchProviders.Domain.Queries
{
  using RoleExpression = Expression<Func<Role, bool>>;

  public static class RoleQueries
  {
    // TODO: Consider casing. See note in UserQueries

    public static RoleExpression RoleByName(string name) {
      return role => role.Name.Equals(name);
    }

    private static RoleExpression RoleContainingUser(User user) {
      return role => role.Users.Contains(user);
    }

    public static IQueryable<Role> WithUser(this IQueryable<Role> roles, User user) {
      return roles.Where(RoleContainingUser(user));
    }

    public static Func<Role, int, string> SelectName {
      get { return (role, i) => role.Name; }
    }
  }
}
