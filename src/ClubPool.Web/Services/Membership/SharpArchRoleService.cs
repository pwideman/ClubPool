using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core;

using ClubPool.Core;
using ClubPool.Core.Queries;
using ClubPool.Framework.NHibernate;

namespace ClubPool.Web.Services.Membership
{
  public class SharpArchRoleService : IRoleService
  {
    protected ILinqRepository<User> userRepository;
    protected ILinqRepository<Role> roleRepository;

    public SharpArchRoleService(ILinqRepository<Role> roleRepo, ILinqRepository<User> userRepo) {
      Check.Require(null != roleRepo, "roleRepo cannot be null");
      Check.Require(null != userRepo, "userRepo cannot be null");

      roleRepository = roleRepo;
      userRepository = userRepo;
    }

    public string[] GetRolesForUser(string username) {
      Check.Require(!string.IsNullOrEmpty(username), "username cannot be null or empty");

      var user = userRepository.FindOne(UserQueries.UserByUsername(username));
      if (null == user) {
        throw new ArgumentException(string.Format("User '{0}' does not exist", username));
      }
      var roles = roleRepository.GetAll().WithUser(user);
      return roles.ToList().Select(RoleQueries.SelectName).ToArray();
    }

    public bool IsUserInRole(string username, string roleName) {
      Check.Require(!string.IsNullOrEmpty(username), "username cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(roleName), "roleName cannot be null or empty");

      var role = roleRepository.FindOne(RoleQueries.RoleByName(roleName));
      if (null != role) {
        return role.Users.AsQueryable().WithUsername(username).Any();
      }
      else {
        return false;
      }
    }

    public string[] GetUsersInRole(string roleName) {
      Check.Require(!string.IsNullOrEmpty(roleName), "roleName cannot be null or empty");

      var role = roleRepository.FindOne(RoleQueries.RoleByName(roleName));
      if (null == role) {
        throw new ArgumentException(string.Format("Role '{0}' does not exist", roleName));
      }
      return role.Users.Select(UserQueries.SelectUsername).ToArray();
    }

    public bool IsUserAdministrator(string username) {
      return IsUserInRole(username, Roles.Administrators);
    }
  }
}
