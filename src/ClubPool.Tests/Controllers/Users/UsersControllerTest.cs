using System;

using Moq;
using NUnit.Framework;

using ClubPool.Testing;
using ClubPool.Web.Controllers.Users;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Services.Messaging;
using ClubPool.Web.Services.Configuration;
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
    protected Mock<IConfigurationService> configService;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      membershipService = new Mock<IMembershipService>();
      emailService = new Mock<IEmailService>();
      configService = new Mock<IConfigurationService>();
      var config = new ClubPoolConfiguration("test", "test", "test@test.com", "test", false);
      configService.Setup(c => c.GetConfig()).Returns(config);
      controller = new UsersController(authenticationService, membershipService.Object,
        emailService.Object, configService.Object, repository.Object);
    }
  }
}
