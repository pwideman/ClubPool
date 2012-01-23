using System;

using Moq;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Divisions;
using ClubPool.Testing;
using ClubPool.Web.Models;

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

    public EditDivisionViewModel CreateEditDivisionViewModel(Division division) {
      var model = new EditDivisionViewModel() {
        Id = division.Id,
        Version = division.EncodedVersion,
        SeasonName = division.Season.Name,
        Name = division.Name,
        StartingDate = division.StartingDate.ToShortDateString()
      };
      return model;
    }
  }
}
