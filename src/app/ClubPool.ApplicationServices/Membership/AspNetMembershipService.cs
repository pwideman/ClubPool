﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using ClubPool.ApplicationServices.Membership.Contracts;

namespace ClubPool.ApplicationServices.Membership
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

    public void CreateUser(string username, string password, string email, bool isApproved) {
      MembershipCreateStatus status;
      var membershipUser = membershipProvider.CreateUser(username, password, email, null, null, isApproved, null, out status);
      if (status != MembershipCreateStatus.Success) {
        throw new MembershipCreateUserException(status);
      }
    }

  }
}