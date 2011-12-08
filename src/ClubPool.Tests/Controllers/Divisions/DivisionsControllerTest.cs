using System;

using Moq;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Divisions;
using ClubPool.Testing;

namespace ClubPool.Tests.Controllers.Divisions
{
  public abstract class DivisionsControllerTest : SpecificationContext
  {
    protected DivisionsController controller;
    protected Mock<IRepository> repository;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      controller = new DivisionsController(repository.Object);
    }
  }
}
