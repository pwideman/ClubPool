using System;
using System.Web.Security;

using NUnit.Framework;

using Tests.ClubPool.SharpArchProviders.TestDoubles;

namespace Tests.ClubPool.SharpArchProviders
{
  [TestFixture]
  public class SharpArchMembershipProviderTestsWithClearPasswordFormat : SharpArchMembershipProviderTestsBase
  {
    protected override TestSharpArchMembershipProvider GetProvider() {
      return Membership.Providers["TestSharpArchMembershipProviderWithClearPasswordFormat"] as TestSharpArchMembershipProvider;
    }
  }

  [TestFixture]
  public class SharpArchMembershipProviderTestsWithHashedPasswordFormat : SharpArchMembershipProviderTestsBase
  {
    protected override TestSharpArchMembershipProvider GetProvider() {
      return Membership.Providers["TestSharpArchMembershipProviderWithHashedPasswordFormat"] as TestSharpArchMembershipProvider;
    }
  }

}
