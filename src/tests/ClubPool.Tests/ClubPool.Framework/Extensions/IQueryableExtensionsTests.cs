using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using SharpArch.Testing.NUnit;
using SharpArch.Core;

using ClubPool.Framework.Extensions;
using Tests.ClubPool.Data.TestDoubles;

namespace Tests.ClubPool.Framework.Extensions
{
  [TestFixture]
  public class IQueryableExtensionsTests
  {

    // Page tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void Page_throws_when_query_is_null() {
      IQueryable<int> query = null;
      
      query.Page(0, 1);
    }

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void Page_throws_when_index_is_negative() {
      var query = new List<int>().AsQueryable();
      
      query.Page(-1, 1);
    }

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void Page_throws_when_size_is_not_greather_than_zero() {
      var query = new List<int>().AsQueryable();
      
      query.Page(1, 0);
    }

    [Test]
    public void Page_returns_correct_size() {
      var users = MockUserRepositoryFactory.CreateUsers(10).AsQueryable();
      
      var list = users.Page(0, 5).ToList();
      
      list.Count.ShouldEqual(5);
    }

    [Test]
    public void Page_returns_correct_index() {
      var users = MockUserRepositoryFactory.CreateUsers(10).AsQueryable();

      var list = users.Page(1, 3).ToList();
      var user = list[0];
      
      user.Id.ShouldEqual(3);
    }

    [Test]
    public void Page_returns_correct_truncated_list_for_last_page() {
      var users = MockUserRepositoryFactory.CreateUsers(3).AsQueryable();

      var list = users.Page(1, 2).ToList();
      var user = list[0];

      list.Count.ShouldEqual(1);
      user.Id.ShouldEqual(2);
    }

    [Test]
    public void Page_returns_all_for_first_page_when_size_is_greather_than_length() {
      var users = MockUserRepositoryFactory.CreateUsers(5).AsQueryable();

      var list = users.Page(0, 6).ToList();

      list.Count.ShouldEqual(5);
    }

    [Test]
    public void Page_returns_empty_list_for_second_page_when_size_is_greater_than_length() {
      var users = MockUserRepositoryFactory.CreateUsers(5).AsQueryable();

      var list = users.Page(1, 6).ToList();

      list.Count.ShouldEqual(0);
    }

  }
}
