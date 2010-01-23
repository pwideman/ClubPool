using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using SharpArch.Testing.NUnit;

using ClubPool.SharpArchProviders.Domain;
using ClubPool.SharpArchProviders.Domain.Queries;

using Tests.ClubPool.SharpArchProviders.TestDoubles;

namespace Tests.ClubPool.SharpArchProviders.Domain.Queries
{
  [TestFixture]
  public class UserQueriesTests
  {
    protected IQueryable<User> users = null;

    [SetUp]
    public void Setup() {
      users = MockUserRepositoryFactory.CreateUsers(5).AsQueryable();
    }

    #region UserByUsername Tests

    [Test]
    public void UserByUsername_returns_correct_user() {
      var user = users.First();

      var testuser = users.SingleOrDefault(UserQueries.UserByUsername(user.Username));

      testuser.ShouldNotBeNull();
      testuser.Username.ShouldEqual(user.Username);
    }

    [Test]
    public void UserByUsername_returns_null_for_invalid_username() {
      users.SingleOrDefault(UserQueries.UserByUsername("junk")).ShouldBeNull();
    }

    #endregion

    #region UserByEmail Tests

    [Test]
    public void UserByEmail_returns_correct_user() {
      var user = users.ToList()[3];

      var testuser = users.SingleOrDefault(UserQueries.UserByEmail(user.Email));

      testuser.Email.ShouldEqual(user.Email);
    }

    [Test]
    public void UserByEmail_returns_null_for_invalid_email() {
      var testuser = users.SingleOrDefault(UserQueries.UserByEmail("junk"));

      testuser.ShouldBeNull();
    }

    #endregion

    #region UserByUsernameContains Tests

    [Test]
    public void UserByUsernameContains_returns_correct_list_for_username() {
      var tempusers = users.ToList();
      var searchstring = "myname";
      tempusers.Add(MockUserRepositoryFactory.CreateTransientUser(searchstring + "1"));
      tempusers.Add(MockUserRepositoryFactory.CreateTransientUser(searchstring + "2"));

      var list = tempusers.AsQueryable().Where(UserQueries.UserByUsernameContains(searchstring)).ToList();

      list.Count.ShouldEqual(2);
      foreach (var user in list) {
        user.Username.ShouldContain(searchstring);
      }
    }

    [Test]
    public void UserByUsernameContains_returns_empty_list_when_no_matches() {
      var list = users.Where(UserQueries.UserByUsernameContains("junk")).ToList();

      list.Count.ShouldEqual(0);
    }

    #endregion

    #region WithUsernameContaining Tests

    [Test]
    public void WithUsernameContaining_returns_empty_for_invalid_username() {
      users.WithUsernameContaining("junk").Count().ShouldEqual(0);
    }

    [Test]
    public void WithUsernameContaining_returns_correct_users_for_username() {
      var testusers = users.Take(3).ToList();
      var search = "_search_";
      foreach (var user in testusers) {
        user.Username += search;
      }

      users.WithUsernameContaining(search).Count().ShouldEqual(testusers.Count());
    }


    #endregion

    #region WithEmailContaining Tests

    [Test]
    public void WithEmailContaining_returns_empty_for_invalid_email() {
      users.WithEmailContaining("junk").Count().ShouldEqual(0);
    }

    [Test]
    public void WithEmailContaining_returns_correct_users_for_email() {
      users.WithEmailContaining("email").Count().ShouldEqual(users.Count());
    }

    #endregion

    #region SelectUsername Tests

    [Test]
    public void SelectUsername_selects_username() {
      var selectedNames = users.Select(UserQueries.SelectUsername).ToArray();

      for (int i = 0; i < selectedNames.Length; i++) {
        users.ElementAt(i).Username.ShouldEqual(selectedNames[i]);
      }
    }

    #endregion

  }
}
