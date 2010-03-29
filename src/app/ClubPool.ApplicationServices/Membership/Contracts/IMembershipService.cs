using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using ClubPool.Core;

namespace ClubPool.ApplicationServices.Membership.Contracts
{
  public interface IMembershipService
  {
    bool ValidateUser(string username, string password);
    User CreateUser(string username, string password, string firstName, string lastName, string email, bool isApproved);
    bool UsernameIsInUse(string username);
    bool EmailIsInUse(string email);
  }
}
