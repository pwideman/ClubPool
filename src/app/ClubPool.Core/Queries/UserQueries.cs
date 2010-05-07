using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ClubPool.Core.Queries
{
  using UserExpression = Expression<Func<User,bool>>;

  public static class UserQueries
  {
    // TODO: Consider casing. Some of these comparisons should be case insensitive,
    // but I don't want to add a bunch of ToLower()'s until I know
    // what NHibernate.Linq will do with that. I also need to see
    // how these perform against a live database. I know that string.Equals()
    // is case sensitive so testing against in memory IQueryable sources will
    // be case sensitive, but when using these against the database maybe
    // the database's case sensitivity settings come into play? Those could
    // possibly be case insensitive by default, etc.

    public static UserExpression UserByUsername(string username) {
      return user => user.Username.Equals(username);
    }

    public static UserExpression UserByEmail(string email) {
      return user => user.Email.Equals(email);
    }

    private static UserExpression UserByEmailContains(string email) {
      return user => user.Email.Contains(email);
    }

    public static IQueryable<User> WithEmailContaining(this IQueryable<User> users, string email) {
      return users.Where(UserByEmailContains(email));
    }

    public static IQueryable<User> WithEmail(this IQueryable<User> users, string email) {
      return users.Where(UserByEmail(email));
    }

    public static UserExpression UserByUsernameContains(string username) {
      return user => user.Username.Contains(username);
    }

    public static IQueryable<User> WithUsernameContaining(this IQueryable<User> users, string username) {
      return users.Where(UserByUsernameContains(username));
    }

    public static IQueryable<User> WithUsernames(this IQueryable<User> users, string[] usernames) {
      return users.Where(u => usernames.Contains(u.Username));
    }

    public static IQueryable<User> WithUsername(this IQueryable<User> users, string username) {
      return users.Where(UserByUsername(username));
    }

    public static IQueryable<User> WhereUnapproved(this IQueryable<User> users) {
      return users.Where(u => !u.IsApproved);
    }

    public static IQueryable<User> WhereIdIn(this IQueryable<User> users, int[] ids) {
      if (null != ids && ids.Length > 0) {
        return users.Where(u => ids.Contains(u.Id));
      }
      else {
        return new User[0].AsQueryable();
      }
    }

    public static Func<User, int, string> SelectUsername {
      get { return (u, i) => u.Username; }
    }
  }
}
