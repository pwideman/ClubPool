using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Configuration.Provider;

using SharpArch.Core;

using ClubPool.Framework.Extensions;
using ClubPool.Framework.NHibernate;

using ClubPool.SharpArchProviders.Domain;
using ClubPool.SharpArchProviders.Domain.Queries;

namespace ClubPool.SharpArchProviders
{
  public class SharpArchRoleProvider : RoleProvider
  {
    protected ILinqRepository<Role> roleRepository;
    protected ILinqRepository<User> userRepository;

    public SharpArchRoleProvider() {
      roleRepository = SafeServiceLocator<ILinqRepository<Role>>.GetService();
      userRepository = SafeServiceLocator<ILinqRepository<User>>.GetService();
    }

    #region RoleProvider/IRoleService implementation

    public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
      Check.Require(null != usernames, "usernames cannot be null");
      Check.Require(null != roleNames, "roleNames cannot be null");

      var roles = new List<Role>();
      var users = new List<User>();

      // validate usernames
      foreach (var username in usernames) {
        Check.Require(!username.IsNullOrEmptyOrBlank(),
          "usernames cannot contain null or empty username");
        var user = userRepository.FindOne(UserQueries.UserByUsername(username));
        if (null == user) {
          throw new ProviderException(string.Format("Username '{0}' does not exist", username));
        }
        users.Add(user);
      }

      // validate rolenames
      foreach (var rolename in roleNames) {
        Check.Require(!rolename.IsNullOrEmptyOrBlank(),
          "roleNames cannot contain null or empty rolename");
        var role = roleRepository.FindOne(RoleQueries.RoleByName(rolename));
        if (null == role) {
          throw new ProviderException(string.Format("Rolename '{0}' does not exist", rolename));
        }
        roles.Add(role);
      }

      foreach (var role in roles) {
        foreach (var user in users) {
          if (!role.Users.Contains(user)) {
            role.Users.Add(user);
          }
        }
        roleRepository.SaveOrUpdate(role);
      }
    }

    public override string ApplicationName {
      get {
        throw new NotImplementedException();
      }
      set {
        throw new NotImplementedException();
      }
    }

    public override void CreateRole(string roleName) {
      Check.Require(!roleName.IsNullOrEmptyOrBlank(), "roleName cannot be null or empty");

      // check for duplicate
      var role = roleRepository.FindOne(RoleQueries.RoleByName(roleName));
      if (null != role) {
        throw new ProviderException(string.Format("There is already a role with the name '{0}'", roleName));
      }

      role = new Role(roleName);
      roleRepository.SaveOrUpdate(role);
    }

    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
      if (roleName.IsNullOrEmptyOrBlank()) {
        return false;
      }

      var role = roleRepository.FindOne(RoleQueries.RoleByName(roleName));
      if (null != role) {
        if (throwOnPopulatedRole && role.Users.Count() > 0) {
          throw new ProviderException("Role is not empty");
        }
        roleRepository.Delete(role);
        return true;
      }
      else {
        return false;
      }
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
      Check.Require(!roleName.IsNullOrEmptyOrBlank(), "roleName cannot be null or empty");

      var role = roleRepository.FindOne(RoleQueries.RoleByName(roleName));
      if (null == role) {
        throw new ProviderException(string.Format("Role '{0}' does not exist", roleName));
      }
      var usernames = role.Users.AsQueryable()
        .Where(UserQueries.UserByUsernameContains(usernameToMatch))
        .Select((u, i) => u.Username).ToList();
      return usernames.ToArray();
    }

    public override string[] GetAllRoles() {
      var roles = roleRepository.GetAll().ToArray();
      var rolenames = new List<string>();
      foreach (var role in roles) {
        rolenames.Add(role.Name);
      }
      return rolenames.ToArray();
    }

    public override string[] GetRolesForUser(string username) {
      Check.Require(!username.IsNullOrEmptyOrBlank(), "username cannot be null or empty");

      var user = userRepository.FindOne(UserQueries.UserByUsername(username));
      if (null == user) {
        throw new ProviderException(string.Format("User '{0}' does not exist", username));
      }
      var roles = roleRepository.GetAll().WithUser(user);
      return roles.ToList().Select(RoleQueries.SelectName).ToArray();
    }

    public override string[] GetUsersInRole(string roleName) {
      Check.Require(!roleName.IsNullOrEmptyOrBlank(), "roleName cannot be null or empty");

      var role = roleRepository.FindOne(RoleQueries.RoleByName(roleName));
      if (null == role) {
        throw new ProviderException(string.Format("Role '{0}' does not exist", roleName));
      }
      return role.Users.Select(UserQueries.SelectUsername).ToArray();
    }

    public override bool IsUserInRole(string username, string roleName) {
      Check.Require(!username.IsNullOrEmptyOrBlank(), "username cannot be null or empty");
      Check.Require(!roleName.IsNullOrEmptyOrBlank(), "roleName cannot be null or empty");

      var role = roleRepository.FindOne(RoleQueries.RoleByName(roleName));
      if (null != role) {
        return role.Users.AsQueryable()
          .Where(UserQueries.UserByUsername(username)).Count() > 0;
      }
      else {
        return false;
      }
    }

    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
      Check.Require(null != usernames && usernames.Length != 0,
        "usernames cannot be null or empty");
      Check.Require(null != roleNames && roleNames.Length != 0,
        "roleNames cannot be null or empty");

      List<User> users = new List<User>();
      foreach (var username in usernames) {
        Check.Require(!username.IsNullOrEmptyOrBlank(),
          "usernames cannot contain null or empty strings");
        var user = userRepository.FindOne(UserQueries.UserByUsername(username));
        if (null == user) {
          throw new ProviderException(string.Format("User '{0}' does not exist", username));
        }
        users.Add(user);
      }
      
      List<Role> roles = new List<Role>();
      foreach (var rolename in roleNames) {
        Check.Require(!rolename.IsNullOrEmptyOrBlank(),
          "roleNames cannot contain null or empty strings");
        var role = roleRepository.FindOne(RoleQueries.RoleByName(rolename));
        if (null == role) {
          throw new ProviderException(string.Format("Role '{0}' does not exist", rolename));
        }
        roles.Add(role);
      }

      foreach (var role in roles) {
        foreach (var user in users) {
          if (!role.Users.Contains(user)) {
            throw new ProviderException(string.Format("User '{0}' is not in role '{0}'",
              user.Username, role.Name));
          }
          role.Users.Remove(user);
          roleRepository.SaveOrUpdate(role);
        }
      }
    }

    public override bool RoleExists(string roleName) {
      return null == roleRepository.FindOne(RoleQueries.RoleByName(roleName)) ? false : true;
    }

    #endregion RoleProvider/IRoleService implementation
  }
}
