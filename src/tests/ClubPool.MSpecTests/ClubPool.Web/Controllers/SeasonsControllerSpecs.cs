using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;

using Rhino.Mocks;
using Machine.Specifications;
using SharpArch.Testing;

using ClubPool.Core;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Seasons;
using ClubPool.Web.Controllers.Seasons.ViewModels;
using ClubPool.Framework.NHibernate;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Seasons
{
  public abstract class specification_for_Seasons_controller
  {
    protected static SeasonsController controller;
    protected static ILinqRepository<Season> seasonsRepository;

    Establish context = () => {
      seasonsRepository = MockRepository.GenerateStub<ILinqRepository<Season>>();
      controller = new SeasonsController(seasonsRepository);
      ControllerHelper.CreateMockControllerContext(controller);
      ServiceLocatorHelper.AddValidator();
    };
  }

  // We don't need to test the paging here, that is tested in PagedListViewModelBaseSpecs
  [Subject(typeof(SeasonsController))]
  public class when_asked_for_the_default_view : specification_for_Seasons_controller
  {
    static ActionResult result;
    static int page = 1;
    static int pages = 3;
    static int pageSize = 10;

    Establish context = () => {
      var seasons = new List<Season>();
      for (var i = 0; i < pages * pageSize; i++) {
        seasons.Add(new Season("season" + i.ToString()));
      }
      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
    };

    Because of = () => result = controller.Index(page);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

  }



}
