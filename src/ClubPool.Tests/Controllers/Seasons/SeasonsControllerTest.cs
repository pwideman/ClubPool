using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Seasons;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Seasons
{
  public abstract class SeasonsControllerTest : SpecificationContext
  {
    protected SeasonsController controller;
    protected Mock<IRepository> repository;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      controller = new SeasonsController(repository.Object);
    }
  }
}
