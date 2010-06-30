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

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
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



}
