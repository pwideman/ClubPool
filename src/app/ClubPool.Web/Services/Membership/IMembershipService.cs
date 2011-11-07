using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using ClubPool.Core;

namespace ClubPool.Web.Services.Membership
{
  public interface IMembershipService
  {
    bool ValidateUser(string username, string password);
    User CreateUser(string username, string password, string firstName, string lastName, string email, bool isApproved, bool isLocked);
    bool UsernameIsInUse(string username);
    bool EmailIsInUse(string email);
    string GeneratePasswordResetToken(User user);
    bool ValidatePasswordResetToken(string token, User user);
    string EncodePassword(string password, string salt);
  }
}
