using System;

using Moq;
using NUnit.Framework;

using ClubPool.Testing;
using ClubPool.Web.Controllers.AccountHelp;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Services.Messaging;
using ClubPool.Web.Services.Configuration;
using ClubPool.Web.Infrastructure.Configuration;

namespace ClubPool.Tests.Controllers.AccountHelp
{
  public class AccountHelpControllerTest : SpecificationContext
  {
    protected AccountHelpController controller;
    protected Mock<IRepository> repository;
    protected MockAuthenticationService authenticationService;
    protected Mock<IMembershipService> membershipService;
    protected Mock<IEmailService> emailService;
    protected Mock<IConfigurationService> configService;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      membershipService = new Mock<IMembershipService>();
      emailService = new Mock<IEmailService>();
      configService = new Mock<IConfigurationService>();
      var config = new ClubPoolConfiguration("test", "test", "test@test.com", "test", false);
      configService.Setup(c => c.GetConfig()).Returns(config);
      controller = new AccountHelpController(authenticationService, membershipService.Object,
        emailService.Object, configService.Object, repository.Object);
    }
  }
}
