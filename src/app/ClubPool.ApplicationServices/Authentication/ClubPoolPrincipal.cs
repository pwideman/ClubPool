using System.Linq;
using System.Security.Principal;

using SharpArch.Core;

namespace ClubPool.ApplicationServices.Authentication
{
  /// <summary>
  /// An implementation of IPrincipal that uses our role service to determine
  /// whether the user identity is in a role
  /// </summary>
  public class ClubPoolPrincipal : IPrincipal
  {
    protected string[] roles;

    /// <summary>
    /// ClubPoolPrincipal
    /// </summary>
    /// <param name="id">This principal's identity</param>
    /// <param name="roleSvc">The role service</param>
    public ClubPoolPrincipal(IIdentity id, string[] roles) {
      Check.Require(null != id, "id cannot be null");

      Identity = id;
      this.roles = roles;
    }

    public IIdentity Identity { get; protected set; }

    public bool IsInRole(string role) {
      if (null != roles && roles.Length > 0) {
        return roles.Contains(role);
      }
      else {
        return false;
      }
    }
  }
}
