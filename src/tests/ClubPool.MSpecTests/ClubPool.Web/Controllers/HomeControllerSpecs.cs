﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using System.Web.Routing;

using Microsoft.Web.Mvc;
using MvcContrib;
using Machine.Specifications;
using Rhino.Mocks;

using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Home.ViewModels;
using ClubPool.Web.Controllers.Shared.SidebarGadgets;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_home_controller
  {
    protected static HomeController controller;

    Establish context = () => {
      controller = new HomeController();
    };
  }

  [Subject(typeof(HomeController))]
  public class when_the_home_controller_is_asked_for_the_default_view_when_user_is_not_logged_in : specification_for_home_controller
  {
    static ActionResult result;

    Establish context = () => {
      var mockIdentity = MockRepository.GenerateMock<IIdentity>();
      mockIdentity.Expect(i => i.IsAuthenticated).Return(false);
      var mockPrincipal = MockRepository.GenerateMock<IPrincipal>();
      mockPrincipal.Expect(p => p.Identity).Return(mockIdentity);
      var mockHttpContext = MockRepository.GenerateMock<HttpContextBase>();
      mockHttpContext.Expect(c => c.User).Return(mockPrincipal);
      var controllerContext = new ControllerContext(mockHttpContext, new RouteData(), controller);
      controller.ControllerContext = controllerContext;
    };

    Because of = () => result = controller.Index();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_add_sidebar_gadget_collection_to_the_view_data = () => {
      var viewResult = result as ViewResult;
      viewResult.ViewData.Contains<SidebarGadgetCollection>().ShouldBeTrue();
    };

    It should_add_login_gadget_to_sidebar_gadget_collection = () => {
      var viewResult = result as ViewResult;
      var sidebarCollection = viewResult.ViewData.Get<SidebarGadgetCollection>();
      sidebarCollection.Count.ShouldEqual(1);
      var loginViewData = sidebarCollection["Login"];
      loginViewData.Name.ShouldEqual("Login");
    };
  }
}