using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using SharpArch.Testing.NUnit;

using ClubPool.SharpArchProviders.Domain;
using ClubPool.SharpArchProviders.Domain.Queries;

using Tests.ClubPool.SharpArchProviders.TestDoubles;

namespace Tests.ClubPool.SharpArchProviders.Domain.Queries
{
  [TestFixture]
  public class RoleQueriesTests
  {
    protected IQueryable<Role> roles = null;

    [SetUp]
    public void Setup() {
      roles = MockRoleRepositoryFactory.CreateRoles(5).AsQueryable();
    }

    #region RoleByName Tests

    [Test]
    public void RoleByName_returns_correct_role() {
      var role = roles.First();

      var testrole = roles.SingleOrDefault(RoleQueries.RoleByName(role.Name));

      testrole.ShouldNotBeNull();
      testrole.Name.ShouldEqual(role.Name);
    }

    [Test]
    public void RoleByName_returns_null_for_invalid_name() {
      roles.SingleOrDefault(RoleQueries.RoleByName("junk")).ShouldBeNull();
    }

    #endregion RoleByName Tests

    #region RoleContainingUser Tests

    [Test]
    public void WithUser_returns_empty_for_invalid_user() {
      var users = MockUserRepositoryFactory.CreateUsers(2);
      var role = roles.First();
      foreach (var user in users) {
        role.Users.Add(user);
      }
      var invaliduser = new User { Username = "junk" };

      roles.WithUser(invaliduser).Count().ShouldEqual(0);
    }

    [Test]
    public void WithUser_returns_correct_roles_for_user() {
      var user = MockUserRepositoryFactory.CreateUser(1);
      var testroles = roles.Take(3).ToList();
      foreach (var role in testroles) {
        role.Users.Add(user);
      }

      var resultroles = roles.WithUser(user);

      resultroles.Count().ShouldEqual(testroles.Count);
      foreach(var resultrole in resultroles) {
        testroles.Contains(resultrole).ShouldBeTrue();
      }
    }

    #endregion RoleContainingUser Tests

    #region SelectName Tests

    [Test]
    public void SelectName_selects_names() {
      var selectNameRoles = roles.Select(RoleQueries.SelectName).ToArray();

      for (int i = 0; i < selectNameRoles.Length; i++) {
        roles.ElementAt(i).Name.ShouldEqual(selectNameRoles[i]);
      }
    }

    #endregion SelectName Tests
  }
}
