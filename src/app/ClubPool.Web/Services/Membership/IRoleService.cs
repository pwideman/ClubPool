using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Services.Membership
{
  public interface IRoleService
  {
    string[] GetRolesForUser(string username);
    bool IsUserInRole(string username, string roleName);
    string[] GetUsersInRole(string roleName);
    bool IsUserAdministrator(string username);
  }
}
