using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using System.Web.Routing;

using Microsoft.Web.Mvc;
using Machine.Specifications;
using Rhino.Mocks;

using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Home;
using ClubPool.Web.Controllers.Home.ViewModels;
using ClubPool.Web.Controllers.Shared.SidebarGadgets;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Configuration.Contracts;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Home
{
  public abstract class specification_for_home_controller
  {
    protected static HomeController controller;
    protected static IAuthenticationService authenticationService;
    protected static IConfigurationService configService;

    Establish context = () => {
      authenticationService = MockRepository.GenerateStub<IAuthenticationService>();
      configService = MockRepository.GenerateStub<IConfigurationService>();
      configService.Stub(s => s.GetConfig()).Return(new Framework.Configuration.ClubPoolConfiguration("test", "test", "test", null, false));

      controller = new HomeController(authenticationService, configService);
      ControllerHelper.CreateMockControllerContext(controller);
    };
  }

  [Subject(typeof(HomeController))]
  public class when_asked_for_the_default_view_when_user_is_not_logged_in : specification_for_home_controller
  {
    static ViewResultHelper<IndexViewModel> resultHelper;

    Establish context = () => {
      authenticationService.Expect(svc => svc.IsLoggedIn()).Return(false);
    };

    Because of = () => resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index());

    It should_add_sidebar_gadget_collection_to_the_view_data = () =>
      resultHelper.SidebarGadgets.ShouldNotBeNull();

    It should_add_login_gadget_to_sidebar_gadget_collection = () =>
      resultHelper.SidebarGadgets.ContainsKey("Login").ShouldBeTrue();
  }

  [Subject(typeof(HomeController))]
  public class when_asked_for_the_default_view_when_user_is_logged_in : specification_for_home_controller
  {
    static ViewResultHelper<IndexViewModel> resultHelper;

    Establish context = () => {
      authenticationService.Expect(svc => svc.IsLoggedIn()).Return(true);
    };

    Because of = () => resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index());

    It should_not_add_login_gadget_to_sidebar_gadget_collection = () =>
      resultHelper.SidebarGadgets.ContainsKey("Login").ShouldBeFalse();
  }

}
