using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using SharpArch.Testing.NUnit;

using ClubPool.Core;
using ClubPool.Core.Queries;

using Tests.ClubPool.Data.TestDoubles;

namespace Tests.ClubPool.Core.Queries
{
  [TestFixture]
  public class PlayerQueriesTests
  {
    protected IQueryable<Player> players = null;

    [SetUp]
    public void Setup() {
      //players = MockPlayerRepositoryFactory.CreatePlayers(5).AsQueryable();
    }

  }
}
