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
using ClubPool.Framework.NHibernate;
using ClubPool.Core;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public class specification_for_dashboard_controller
  {
    protected static DashboardController controller;
    protected static IRoleService roleService;
    protected static IAuthenticationService authenticationService;
    protected static ILinqRepository<User> userRepository;

    Establish context = () => {
      roleService = MockRepository.GenerateStub<IRoleService>();
      authenticationService = MockRepository.GenerateStub<IAuthenticationService>();
      userRepository = MockRepository.GenerateStub<ILinqRepository<User>>();

      controller = new DashboardController(roleService, authenticationService, userRepository);
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
      userRepository.Stub(r => r.GetAll()).Return(new List<User>().AsQueryable());
    };

    Because of = () => result = controller.Index();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as IndexViewModel;
      viewModel.UserIsAdmin.ShouldBeTrue();
    };
  }

  [Subject(typeof(DashboardController))]
  public class when_the_dashboard_controller_is_asked_for_the_default_view_for_admin_user_and_there_are_unapproved_users : specification_for_dashboard_controller
  {
    static ActionResult result;
    static string username = "test";
    static IList<User> users;

    Establish context = () => {
      var identity = new Core.Identity() { Username = username };
      authenticationService.Stub(s => s.GetCurrentIdentity()).Return(identity);
      roleService.Stub(s => s.IsUserAdministrator(username)).Return(true);
      users = new List<User>() {
        new User("user1", "user1", "user", "one", "test@test.com") { IsApproved = false },
        new User("user2", "user2", "user", "two", "two@two.com") { IsApproved = false }
      };
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = controller.Index();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as IndexViewModel;
      viewModel.UserIsAdmin.ShouldBeTrue();
      // TODO: Check for alerts
    };
  }
}
