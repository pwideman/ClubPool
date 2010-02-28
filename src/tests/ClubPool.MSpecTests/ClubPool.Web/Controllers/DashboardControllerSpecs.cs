using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Rhino.Mocks;
using Machine.Specifications;

using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Dashboard.ViewModels;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Membership.Contracts;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public class specification_for_dashboard_controller
  {
    protected static DashboardController controller;
    protected static IRoleService roleService;
    protected static IAuthenticationService authenticationService;

    Establish context = () => {
      roleService = MockRepository.GenerateStub<IRoleService>();
      authenticationService = MockRepository.GenerateStub<IAuthenticationService>();

      controller = new DashboardController(roleService, authenticationService);
      ControllerHelper.CreateMockControllerContext(controller);
    };
  }

  [Subject(typeof(DashboardController))]
  public class when_the_dashboard_controller_is_asked_for_the_default_view_for_nonadmin_user : specification_for_dashboard_controller
  {
    static ActionResult result;
    static string username = "test";

    Establish context = () => {
      var identity = new Core.Identity() { Username = username };
      authenticationService.Stub(s => s.GetCurrentIdentity()).Return(identity);
      roleService.Stub(s => s.IsUserAdministrator(username)).Return(false);
    };

    Because of = () => result = controller.Index();

    It should_ask_for_the_current_identity = () =>
      authenticationService.AssertWasCalled(s => s.GetCurrentIdentity());

    It should_ask_if_the_user_is_admin = () =>
      roleService.AssertWasCalled(s => s.IsUserAdministrator(username));

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as IndexViewModel;
      viewModel.UserIsAdmin.ShouldBeFalse();
    };
  }

  [Subject(typeof(DashboardController))]
  public class when_the_dashboard_controller_is_asked_for_the_default_view_for_admin_user : specification_for_dashboard_controller
  {
    static ActionResult result;
    static string username = "test";

    Establish context = () => {
      var identity = new Core.Identity() { Username = username };
      authenticationService.Stub(s => s.GetCurrentIdentity()).Return(identity);
      roleService.Stub(s => s.IsUserAdministrator(username)).Return(true);
    };

    Because of = () => result = controller.Index();

    It should_ask_for_the_current_identity = () =>
      authenticationService.AssertWasCalled(s => s.GetCurrentIdentity());

    It should_ask_if_the_user_is_admin = () =>
      roleService.AssertWasCalled(s => s.IsUserAdministrator(username));

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as IndexViewModel;
      viewModel.UserIsAdmin.ShouldBeTrue();
    };
  }
}
