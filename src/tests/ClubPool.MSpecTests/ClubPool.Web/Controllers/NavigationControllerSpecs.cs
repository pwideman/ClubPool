using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;

using Rhino.Mocks;
using Machine.Specifications;

using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.Core;
using ClubPool.Web.Controllers.Navigation;
using ClubPool.Web.Controllers.Navigation.ViewModels;
using ClubPool.Testing.ApplicationServices.Authentication;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_navigation_controller
  {
    protected static NavigationController controller;
    protected static MockAuthenticationService authService;

    Establish context = () => {
      authService = AuthHelper.CreateMockAuthenticationService();
      controller = new NavigationController(authService);
      ControllerHelper.CreateMockControllerContext(controller);
    };

  }

  [Subject(typeof(NavigationController))]
  public class when_asked_for_the_menu_when_user_is_not_logged_in : specification_for_navigation_controller
  {
    static ActionResult result;

    Establish context = () => {
      authService.MockPrincipal.MockIdentity.IsAuthenticated = false;
    };

    Because of = () => result = controller.Menu();

    It should_return_the_default_view = () => result.IsAPartialViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_property_to_a_new_menu_view_model = () =>
      result.IsAPartialViewAnd().ViewData.Model.ShouldBeOfType(typeof(MenuViewModel));

    It should_set_the_properties_of_the_view_model_correctly = () => {
      MenuViewModel viewModel = result.IsAPartialViewAnd().ViewData.Model as MenuViewModel;
      viewModel.DisplayAdminMenu.ShouldBeFalse();
      viewModel.UserIsLoggedIn.ShouldBeFalse();
    };
  }

  [Subject(typeof(NavigationController))]
  public class when_asked_for_the_menu_when_normal_user_is_logged_in : specification_for_navigation_controller
  {
    static ActionResult result;

    Establish context = () => {
      var principal = authService.MockPrincipal;
      principal.MockIdentity.IsAuthenticated = true;
    };

    Because of = () => result = controller.Menu();

    It should_return_the_default_view = () => result.IsAPartialViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_property_to_a_new_menu_view_model = () =>
      result.IsAPartialViewAnd().ViewData.Model.ShouldBeOfType(typeof(MenuViewModel));

    It should_set_the_properties_of_the_view_model_correctly = () => {
      MenuViewModel viewModel = result.IsAPartialViewAnd().ViewData.Model as MenuViewModel;
      viewModel.DisplayAdminMenu.ShouldBeFalse();
      viewModel.UserIsLoggedIn.ShouldBeTrue();
    };
  }

  [Subject(typeof(NavigationController))]
  public class when_asked_for_the_menu_when_admin_user_is_logged_in : specification_for_navigation_controller
  {
    static ActionResult result;

    Establish context = () => {
      var principal = authService.MockPrincipal;
      principal.MockIdentity.IsAuthenticated = true;
      principal.Roles = new string[] { Roles.Administrators };
    };

    Because of = () => result = controller.Menu();

    It should_return_the_default_view = () => result.IsAPartialViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_property_to_a_new_menu_view_model = () =>
      result.IsAPartialViewAnd().ViewData.Model.ShouldBeOfType(typeof(MenuViewModel));

    It should_set_the_properties_of_the_view_model_correctly = () => {
      MenuViewModel viewModel = result.IsAPartialViewAnd().ViewData.Model as MenuViewModel;
      viewModel.DisplayAdminMenu.ShouldBeTrue();
      viewModel.UserIsLoggedIn.ShouldBeTrue();
    };
  }
}
