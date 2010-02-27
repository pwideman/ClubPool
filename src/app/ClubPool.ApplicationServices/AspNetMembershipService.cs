using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using ClubPool.ApplicationServices.Contracts;

namespace ClubPool.ApplicationServices
{
  public class AspNetMembershipService : IMembershipService
  {
    protected MembershipProvider membershipProvider;

    public AspNetMembershipService(MembershipProvider provider) {
      membershipProvider = provider;
    }

    public bool ValidateUser(string username, string password) {
      return membershipProvider.ValidateUser(username, password);
    }

    public MembershipUser CreateUser(string username, string password, string email, bool isApproved, out MembershipCreateStatus status) {
      return membershipProvider.CreateUser(username, password, email, null, null, isApproved, null, out status);
    }

  }
}
