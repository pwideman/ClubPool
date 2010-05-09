using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

using ClubPool.ApplicationServices.Membership.Contracts;
using SharpArch.Core;

namespace ClubPool.ApplicationServices.Authentication
{
  /// <summary>
  /// An implementation of IPrincipal that uses our role service to determine
  /// whether the user identity is in a role
  /// </summary>
  public class ClubPoolPrincipal : IPrincipal
  {
    IRoleService roleService;
    IIdentity identity;

    /// <summary>
    /// ClubPoolPrincipal
    /// </summary>
    /// <param name="id">This principal's identity</param>
    /// <param name="roleSvc">The role service</param>
    public ClubPoolPrincipal(IIdentity id, IRoleService roleSvc) {
      Check.Require(null != id, "id cannot be null");
      Check.Require(null != roleSvc, "roleSvc cannot be null");

      identity = id;
      roleService = roleSvc;
    }

    public IIdentity Identity {
      get { return identity; }
    }

    public bool IsInRole(string role) {
      return roleService.IsUserInRole(identity.Name, role);
    }
  }
}
