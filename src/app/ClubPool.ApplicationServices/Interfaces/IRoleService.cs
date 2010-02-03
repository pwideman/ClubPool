using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.ApplicationServices.Interfaces
{
  public interface IRoleService
  {
    string[] GetRolesForUser(string username);
    bool IsUserInRole(string username, string role);
  }
}
