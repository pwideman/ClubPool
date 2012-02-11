using System;

using Moq;
using NUnit.Framework;

using ClubPool.Testing;
using ClubPool.Web.Controllers.Users;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Services.Messaging;
using ClubPool.Web.Infrastructure.Configuration;

namespace ClubPool.Tests.Controllers.Users
{
  public class UsersControllerTest : SpecificationContext
  {
    protected UsersController controller;
    protected Mock<IRepository> repository;
    protected MockAuthenticationService authenticationService;
    protected Mock<IMembershipService> membershipService;
    protected Mock<IEmailService> emailService;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      membershipService = new Mock<IMembershipService>();
      emailService = new Mock<IEmailService>();
      var config = new ClubPoolConfiguration {
        SiteName = "test",
        SmtpHost = "test",
        SystemEmailAddress = "test@test.com",
        SystemEmailPassword = "test",
        UseRescues = false
      };
      controller = new UsersController(authenticationService, membershipService.Object,
        emailService.Object, config, repository.Object);
    }
  }
}
