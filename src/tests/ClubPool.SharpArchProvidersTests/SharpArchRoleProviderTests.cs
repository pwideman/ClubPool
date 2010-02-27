using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Diagnostics;
using System.Configuration.Provider;

using Microsoft.Practices.ServiceLocation;
using Rhino.Mocks;
using NUnit.Framework;
using SharpArch.Testing.NHibernate;
using SharpArch.Testing.NUnit;
using SharpArch.Testing.NUnit.NHibernate;
using SharpArch.Core;
using SharpArch.Data.NHibernate;

using ClubPool.Framework.NHibernate;
using ClubPool.SharpArchProviders;
using ClubPool.SharpArchProviders.Domain;
using ClubPool.SharpArchProviders.Domain.Queries;

using Tests.ClubPool.SharpArchProviders.TestDoubles;

namespace Tests.ClubPool.SharpArchProviders
{
  public class SharpArchRoleProviderTests : ProviderTestsBase
  {
    protected TestSharpArchRoleProvider provider;

    protected override void InitializeProvider() {
      provider = Roles.Provider as TestSharpArchRoleProvider;
    }

    protected override void LoadTestData() {
      Debug.Print("Loading test data");
      for (int i = 0; i < 3; i++) {
        var role = new Role();
        role.Name = "role" + i.ToString();
        provider.RoleRepository.SaveOrUpdate(role);
      }
      for (int i = 0; i < 5; i++) {
        var username = "user" + i.ToString();
        var user = new User(username, username, username + "@email.com");
        provider.UserRepository.SaveOrUpdate(user);
      }
      FlushAndClearSession();
      Debug.Print("Finished loading test data");
    }

    #region AddUsersToRoles Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void AddUsersToRoles_throws_on_null_usernames() {
      provider.AddUsersToRoles(null, new[] {"role1"});
    }

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void AddUsersToRoles_throws_on_null_roleNames() {
      provider.AddUsersToRoles(new[] { "user1" }, null);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void AddUsersToRoles_throws_on_invalid_username() {
      var role = provider.RoleRepository.GetAll().First().Name;
      provider.AddUsersToRoles(new[] { "junk" }, new[] { role });
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void AddUsersToRoles_throws_on_invalid_rolename() {
      var user = provider.UserRepository.GetAll().First().Username;
      provider.AddUsersToRoles(new[] { user }, new[] { "junk" });
    }

    [Test]
    public void AddUsersToRoles_can_add_users_to_roles() {
      List<string> rolenames = new List<string>();
      foreach (var role in provider.RoleRepository.GetAll().ToList()) {
        rolenames.Add(role.Name);
      }
      List<string> usernames = new List<string>();
      foreach (var user in provider.UserRepository.GetAll().ToList()) {
        usernames.Add(user.Username);
      }
      FlushAndClearSession();

      provider.AddUsersToRoles(usernames.ToArray(), rolenames.ToArray());

      FlushAndClearSession();
      // verify that the roles now contain all these users
      foreach (var role in provider.RoleRepository.GetAll().ToList()) {
        foreach (var username in usernames) {
          role.Users.AsQueryable()
            .SingleOrDefault(UserQueries.UserByUsername(username))
            .ShouldNotBeNull(string.Format("role {0} does not contain user {1}", role.Name, username));
        }
      }

      // also verify that the users now contain all these roles
      foreach (var user in provider.UserRepository.GetAll().ToList()) {
        foreach (var rolename in rolenames) {
          user.Roles.AsQueryable()
            .SingleOrDefault(RoleQueries.RoleByName(rolename))
            .ShouldNotBeNull(string.Format("user {0} does not contain role {1}", user.Username, rolename));
        }
      }
    }

    #endregion AddUsersToRoles Tests

    #region CreateRole Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void CreateRole_throws_on_missing_name() {
      provider.CreateRole(null);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void CreateRole_throws_on_duplicate_name() {
      var role = provider.RoleRepository.GetAll().First();

      provider.CreateRole(role.Name);
    }

    [Test]
    public void CreateRole_can_create_role() {
      var newRoleName = "new role";

      provider.CreateRole(newRoleName);
      FlushAndClearSession();

      provider
        .RoleRepository
        .FindOne(RoleQueries.RoleByName(newRoleName))
        .ShouldNotBeNull();
    }

    #endregion CreateRole Tests

    #region DeleteRole Tests

    [Test]
    public void DeleteRole_returns_false_on_missing_name() {
      provider.DeleteRole(null, false).ShouldBeFalse();
    }

    [Test]
    public void DeleteRole_returns_false_on_invalid_name() {
      provider.DeleteRole("junk", false).ShouldBeFalse();
    }

    [Test]
    public void DeleteRole_can_delete_role() {
      var role = provider.RoleRepository.GetAll().First();

      provider.DeleteRole(role.Name, false).ShouldBeTrue();
      FlushAndClearSession();

      provider.RoleRepository.FindOne(RoleQueries.RoleByName(role.Name)).ShouldBeNull();
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void DeleteRole_honors_throwOnPopulatedRole_when_true() {
      var role = provider.RoleRepository.GetAll().First();
      var user = provider.UserRepository.GetAll().First();
      role.Users.Add(user);
      provider.RoleRepository.SaveOrUpdate(role);
      FlushAndClearSession();

      provider.DeleteRole(role.Name, true);
    }

    [Test]
    public void DeleteRole_honors_throwOnPopulatedRole_when_false() {
      var role = provider.RoleRepository.GetAll().First();
      var user = provider.UserRepository.GetAll().First();
      role.Users.Add(user);
      provider.RoleRepository.SaveOrUpdate(role);
      FlushAndClearSession();

      provider.DeleteRole(role.Name, false).ShouldBeTrue();
      FlushAndClearSession();

      provider.RoleRepository.FindOne(RoleQueries.RoleByName(role.Name)).ShouldBeNull();
    }

    #endregion DeleteRole Tests

    #region FindUsersInRole Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void FindUsersInRole_throws_on_null_roleName() {
      provider.FindUsersInRole(null, "junk");
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void FindUsersInRole_throws_on_invalid_roleName() {
      provider.FindUsersInRole("junk", "junk");
    }

    [Test]
    public void FindUsersInRole_returns_empty_array_when_username_not_found() {
      var role = provider.RoleRepository.GetAll().First().Name;
      var usernames = provider.FindUsersInRole(role, null);

      usernames.ShouldBeOfType(typeof(string[]));
      usernames.Length.ShouldEqual(0);
    }

    [Test]
    public void FindUsersInRole_returns_correct_usernames_for_usernameToMatch() {
      var search = "_search_";
      var numUsers = 2;
      for (int i = 0; i < numUsers; i++) {
        var user = new User("junk" + search + i.ToString(), "junk", "junk");
        provider.UserRepository.SaveOrUpdate(user);
      }
      FlushAndClearSession();

      var role = provider.RoleRepository.GetAll().First();
      foreach (var user in provider.UserRepository.GetAll().ToList()) {
        role.Users.Add(user);
      }
      provider.RoleRepository.SaveOrUpdate(role);
      FlushAndClearSession();

      var usernames = provider.FindUsersInRole(role.Name, search);

      usernames.Length.ShouldEqual(numUsers);
      foreach (var username in usernames) {
        username.ShouldContain(search);
      }
    }

    #endregion FindUsersInRole Tests

    #region GetAllRoles Tests

    [Test]
    public void GetAllRoles_returns_empty_array_when_there_are_no_roles() {
      var roles = provider.RoleRepository.GetAll().ToList();
      foreach (var role in roles) {
        provider.RoleRepository.Delete(role);
      }
      FlushAndClearSession();

      var rolenames = provider.GetAllRoles();

      rolenames.ShouldNotBeNull();
      rolenames.Length.ShouldEqual(0);
    }

    [Test]
    public void GetAllRoles_returns_all_roles() {
      var roles = provider.RoleRepository.GetAll().ToList();

      var rolenames = provider.GetAllRoles();

      rolenames.Length.ShouldEqual(roles.Count);
      foreach (var role in roles) {
        rolenames.Where(name => name.Equals(role.Name)).ShouldNotBeNull();
      }
    }

    #endregion GetAllRoles Tests

    #region GetRolesForUser Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void GetRolesForUser_throws_on_null_username() {
      provider.GetRolesForUser(null);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void GetRolesForUser_throws_on_invalid_username() {
      var rolenames = provider.GetRolesForUser("junk");
    }

    [Test]
    public void GetRolesForUser_returns_empty_array_for_user_in_zero_roles() {
      var username = provider.UserRepository.GetAll().First().Username;

      var rolenames = provider.GetRolesForUser(username);

      rolenames.ShouldNotBeNull();
      rolenames.Length.ShouldEqual(0);
    }

    [Test]
    public void GetRolesForUser_returns_correct_roles_for_username() {
      var user = provider.UserRepository.GetAll().First();
      foreach (var role in provider.RoleRepository.GetAll()) {
        role.Users.Add(user);
        provider.RoleRepository.SaveOrUpdate(role);
      }
      FlushAndClearSession();

      var rolenames = provider.GetRolesForUser(user.Username);

      rolenames.ShouldNotBeNull();
      var roles = provider.RoleRepository.GetAll().ToList();
      rolenames.Length.ShouldEqual(roles.Count);
      foreach (var role in roles) {
        role.Users.Where(u => u.Equals(user)).Count().ShouldEqual(1);
      }
    }

    #endregion GetRolesForUser Tests

    #region GetUsersInRole Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void GetUsersInRole_throws_on_null_role() {
      provider.GetUsersInRole(null);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void GetUsersInRole_throws_on_invalid_role() {
      provider.GetUsersInRole("junk");
    }

    [Test]
    public void GetUsersInRole_returns_empty_array_for_empty_role() {
      var role = provider.RoleRepository.GetAll().First().Name;

      var usernames = provider.GetUsersInRole(role);

      usernames.ShouldNotBeNull();
      usernames.Length.ShouldEqual(0);
    }

    [Test]
    public void GetUsersInRole_returns_usernames_in_role() {
      var users = provider.UserRepository.GetAll()
        .Take(3).ToList();
      var role = provider.RoleRepository.GetAll().First();
      foreach (var user in users) {
        role.Users.Add(user);
      }
      provider.RoleRepository.SaveOrUpdate(role);
      FlushAndClearSession();

      var usernames = provider.GetUsersInRole(role.Name);

      usernames.ShouldNotBeNull();
      usernames.Length.ShouldEqual(users.Count);
      foreach (var user in users) {
        usernames.Where(username => username.Equals(user.Username)).Count().ShouldEqual(1);
      }
    }

    #endregion GetUsersInRole Tests

    #region IsUserInRole Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void IsUserInRole_throws_on_null_username() {
      provider.IsUserInRole(null, "junk");
    }

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void IsUserInRole_throws_on_null_rolename() {
      provider.IsUserInRole("junk", null);
    }

    [Test]
    public void IsUserInRole_returns_false_for_nonexistent_username() {
      var rolename = provider.RoleRepository.GetAll().First().Name;

      provider.IsUserInRole("junk", rolename).ShouldBeFalse();
    }

    [Test]
    public void IsUserInRole_returns_false_for_nonexistent_rolename() {
      var username = provider.UserRepository.GetAll().First().Username;

      provider.IsUserInRole(username, "junk").ShouldBeFalse();
    }

    [Test]
    public void IsUserInRole_returns_true_when_user_in_role() {
      var role = provider.RoleRepository.GetAll().First();
      var user = provider.UserRepository.GetAll().First();
      role.Users.Add(user);
      provider.RoleRepository.SaveOrUpdate(role);
      FlushAndClearSession();

      provider.IsUserInRole(user.Username, role.Name).ShouldBeTrue();
    }

    [Test]
    public void IsUserInRole_returns_false_when_user_not_in_role() {
      var rolename = provider.RoleRepository.GetAll().First().Name;
      var username = provider.UserRepository.GetAll().First().Username;

      provider.IsUserInRole(username, rolename).ShouldBeFalse();
    }

    #endregion IsUserInRole Tests

    #region RemoveUsersFromRoles Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void RemoveUsersFromRoles_throws_on_null_usernames() {
      var rolename = provider.RoleRepository.GetAll().First().Name;

      provider.RemoveUsersFromRoles(null, new string[] { rolename });
    }

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void RemoveUsersFromRoles_throws_on_null_rolenames() {
      var username = provider.UserRepository.GetAll().First().Username;

      provider.RemoveUsersFromRoles(new string[] { username }, null);
    }

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void RemoveUsersFromRoles_throws_when_usernames_contains_empty_string() {
      var username = provider.UserRepository.GetAll().First().Username;
      var rolename = provider.RoleRepository.GetAll().First().Name;

      provider.RemoveUsersFromRoles(new string[] { username, "" }, new string[] { rolename });
    }

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void RemoveUsersFromRoles_throws_when_rolenames_contains_empty_string() {
      var username = provider.UserRepository.GetAll().First().Username;
      var rolename = provider.RoleRepository.GetAll().First().Name;

      provider.RemoveUsersFromRoles(new string[] { username }, new string[] { rolename, "" });
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void RemoveUsersFromRoles_throws_when_usernames_contains_invalid_username() {
      var rolename = provider.RoleRepository.GetAll().First().Name;

      provider.RemoveUsersFromRoles(new string[] { "junk" },
        new string[] { rolename });
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void RemoveUsersFromRoles_throws_when_rolenames_contains_invalid_rolename() {
      var username = provider.UserRepository.GetAll().First().Username;

      provider.RemoveUsersFromRoles(new string[] { username },
        new string[] { "junk" });
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void RemoveUsersFromRoles_throws_when_user_not_in_role() {
      var username = provider.UserRepository.GetAll().First().Username;
      var rolename = provider.RoleRepository.GetAll().First().Name;

      provider.RemoveUsersFromRoles(new string[] { username },
        new string[] { rolename });
    }

    [Test]
    public void RemoveUsersFromRoles_removes_users_from_roles() {
      var users = provider.UserRepository.GetAll().ToList();
      var roles = provider.RoleRepository.GetAll().ToList();
      foreach (var role in roles) {
        foreach (var user in users) {
          role.Users.Add(user);
        }
        provider.RoleRepository.SaveOrUpdate(role);
      }

      var usernames = users.AsQueryable().Select(UserQueries.SelectUsername).ToArray();
      var rolenames = roles.AsQueryable().Select(RoleQueries.SelectName).ToArray();
      FlushAndClearSession();

      provider.RemoveUsersFromRoles(usernames, rolenames);

      FlushAndClearSession();
      users = provider.UserRepository.GetAll().ToList();
      foreach (var user in users) {
        user.Roles.Count.ShouldEqual(0);
      }
      roles = provider.RoleRepository.GetAll().ToList();
      foreach (var role in roles) {
        role.Users.Count.ShouldEqual(0);
      }
    }


    #endregion

    #region RoleExists Tests

    [Test]
    public void RoleExists_returns_false_for_null() {
      provider.RoleExists(null).ShouldBeFalse();
    }

    [Test]
    public void RoleExists_returns_false_for_nonexistent_role() {
      provider.RoleExists("junk").ShouldBeFalse();
    }

    [Test]
    public void RoleExists_returns_true_for_existing_role() {
      var rolename = provider.RoleRepository.GetAll().First().Name;

      provider.RoleExists(rolename).ShouldBeTrue();
    }

    #endregion
  }
}
