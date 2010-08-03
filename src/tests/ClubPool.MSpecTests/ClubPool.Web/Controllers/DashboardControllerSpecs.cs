using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Security.Principal;

using Rhino.Mocks;
using Machine.Specifications;

using ClubPool.Web.Controllers.Dashboard;
using ClubPool.Web.Controllers.Dashboard.ViewModels;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Testing.ApplicationServices.Authentication;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public class specification_for_dashboard_controller
  {
    protected static DashboardController controller;
    protected static MockAuthenticationService authenticationService;
    protected static IUserRepository userRepository;

    Establish context = () => {
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      userRepository = MockRepository.GenerateStub<IUserRepository>();

      controller = new DashboardController(authenticationService, userRepository);
      ControllerHelper.CreateMockControllerContext(controller);
    };
  }

  [Subject(typeof(DashboardController))]
  public class when_asked_for_the_default_view_for_nonadmin_user : specification_for_dashboard_controller
  {
    static ActionResult result;

    Establish context = () => {
    };

    Because of = () => result = controller.Index();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as IndexViewModel;
      viewModel.UserIsAdmin.ShouldBeFalse();
    };
  }

  [Subject(typeof(DashboardController))]
  public class when_asked_for_the_default_view_for_admin_user : specification_for_dashboard_controller
  {
    static ActionResult result;

    Establish context = () => {
      var principal = authenticationService.MockPrincipal;
      principal.Roles = new string[] { Roles.Administrators };
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
  public class when_asked_for_the_default_view_for_admin_user_and_there_are_unapproved_users : specification_for_dashboard_controller
  {
    static ActionResult result;
    static IList<User> users;

    Establish context = () => {
      var principal = authenticationService.MockPrincipal;
      principal.MockIdentity.IsAuthenticated = true;
      principal.Roles = new string[] { Roles.Administrators };
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
