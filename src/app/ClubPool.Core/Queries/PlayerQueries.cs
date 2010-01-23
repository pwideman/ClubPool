using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using SharpArch.Core;

using ClubPool.Framework.Extensions;

namespace ClubPool.Core.Queries
{
  using PlayerExpression = Expression<Func<Player,bool>>;

  public static class PlayerQueries
  {
    // TODO: Consider casing. These comparisons should be case insensitive,
    // but I don't want to add a bunch of ToLower()'s until I know
    // what NHibernate.Linq will do with that. I also need to see
    // how these perform against a live database. I know that string.Equals()
    // is case sensitive so testing against in memory IQueryable sources will
    // be case sensitive, but when using these against the database maybe
    // the database's case sensitivity settings come into play? Those could
    // possibly be case insensitive by default, etc.

  }
}
