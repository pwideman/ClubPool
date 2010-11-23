using System.Linq;
using System.Security.Principal;

using SharpArch.Core;
using ClubPool.Core;

namespace ClubPool.ApplicationServices.Authentication
{
  /// <summary>
  /// An implementation of IPrincipal that uses our role service to determine
  /// whether the user identity is in a role
  /// </summary>
  public class ClubPoolPrincipal : IPrincipal
  {
    protected string[] roles;
    public int UserId { get; protected set; }
    public string Username { get; protected set; }
    public IIdentity Identity { get; protected set; }

    protected ClubPoolPrincipal() {
    }

    /// <summary>
    /// ClubPoolPrincipal
    /// </summary>
    /// <param name="id">This principal's identity</param>
    /// <param name="roleSvc">The role service</param>
    public ClubPoolPrincipal(User user, IIdentity identity) {
      Check.Require(null != user, "user cannot be null");
      Check.Require(null != identity, "identity cannot be null");

      Identity = identity;
      UserId = user.Id;
      Username = user.Username;
      roles = user.Roles.Select(r => r.Name).ToArray();
    }

    public bool IsInRole(string role) {
      if (null != roles && roles.Length > 0) {
        return roles.Contains(role);
      }
      else {
        return false;
      }
    }

    public static ClubPoolPrincipal CreateUnauthorizedPrincipal() {
      var principal = new ClubPoolPrincipal();
      principal.Identity = new GenericIdentity("");
      principal.roles = new string[0];
      return principal;
    }
  }
}
