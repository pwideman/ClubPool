using System;

using Moq;
using NUnit.Framework;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Teams;

namespace ClubPool.Tests.Controllers.Teams
{
  public class TeamsControllerTest : SpecificationContext
  {
    protected TeamsController controller;
    protected Mock<IRepository> repository;
    protected MockAuthenticationService authService;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      authService = AuthHelper.CreateMockAuthenticationService();
      controller = new TeamsController(repository.Object, authService);
    }
  }
}
