using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using ClubPool.ApplicationServices.Membership.Contracts;

namespace ClubPool.ApplicationServices.Membership
{
  public class AspNetRoleService : IRoleService
  {
    protected RoleProvider roleProvider;

    public AspNetRoleService(RoleProvider provider) {
      roleProvider = provider;
    }

    #region IRoleService Members

    public string[] GetRolesForUser(string username) {
      return roleProvider.GetRolesForUser(username);
    }

    public bool IsUserInRole(string username, string role) {
      return roleProvider.IsUserInRole(username, role);
    }

    #endregion
  }
}
