using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;

using ClubPool.ApplicationServices.Membership.Contracts;

namespace ClubPool.Web.Controllers.Attributes
{
  public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
  {
    public IRoleService RoleService { get; set; }

    protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext) {
      if (httpContext == null) {
        throw new ArgumentNullException("httpContext");
      }

      IPrincipal user = httpContext.User;
      if (!user.Identity.IsAuthenticated) {
        return false;
      }

      var users = SplitString(Users);
      if (users.Length > 0 && !users.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase)) {
        return false;
      }

      var roles = SplitString(Roles);
      if (roles.Length > 0 && !roles.Any(role => RoleService.IsUserInRole(user.Identity.Name, role))) {
        return false;
      }

      return true;
    }

    internal static string[] SplitString(string original) {
      if (String.IsNullOrEmpty(original)) {
        return new string[0];
      }

      var split = from piece in original.Split(',')
                  let trimmed = piece.Trim()
                  where !String.IsNullOrEmpty(trimmed)
                  select trimmed;
      return split.ToArray();
    }
  }
}
