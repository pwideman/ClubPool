using System;
using System.Security.Principal;

using Rhino.Mocks;

using ClubPool.ApplicationServices.Authentication;
using ClubPool.ApplicationServices.Authentication.Contracts;

namespace ClubPool.Testing.ApplicationServices.Authentication
{
  public static class AuthHelper
  {
    public static MockAuthenticationService CreateMockAuthenticationService() {
      var identity = new MockIdentity("user", true, null);
      var principal = new MockClubPoolPrincipal(identity, null);
      return new MockAuthenticationService(principal);
    }
  }

  public class MockAuthenticationService : IAuthenticationService
  {

    public MockAuthenticationService(MockClubPoolPrincipal principal) {
      MockPrincipal = principal;
    }

    public bool IsLoggedIn() {
      return MockPrincipal.Identity.IsAuthenticated;
    }

    public void LogIn(string userName, bool createPersistentCookie) {
      MockPrincipal.MockIdentity.Name = userName;
      MockPrincipal.MockIdentity.IsAuthenticated = true;
    }

    public void LogOut() {
      MockPrincipal.MockIdentity.Name = null;
      MockPrincipal.MockIdentity.IsAuthenticated = false;
      MockPrincipal.Roles = null;
    }

    public IPrincipal GetCurrentPrincipal() {
      return MockPrincipal;
    }

    public MockClubPoolPrincipal MockPrincipal { get; set; }
  }

  public class MockClubPoolPrincipal : ClubPoolPrincipal
  {
    public MockClubPoolPrincipal(MockIdentity identity, string[] roles)
      : base(identity, roles) {
        MockIdentity = identity;
    }

    public MockIdentity MockIdentity { get; set; }

    public string[] Roles {
      get {
        return this.roles;
      }
      set {
        this.roles = value;
      }
    }
  }

  public class MockIdentity : IIdentity
  {
    public MockIdentity(string name, bool isAuthenticated, string authenticationType) {
      Name = name;
      IsAuthenticated = isAuthenticated;
      AuthenticationType = authenticationType;
    }

    public string AuthenticationType { get; set; }
    public string Name { get; set; }
    public bool IsAuthenticated { get; set; }
  }
}
