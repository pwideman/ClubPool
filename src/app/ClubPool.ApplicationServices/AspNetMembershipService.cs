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

    #region IMembershipService Members

    public bool ValidateUser(string username, string password) {
      return membershipProvider.ValidateUser(username, password);
    }

    #endregion
  }
}
