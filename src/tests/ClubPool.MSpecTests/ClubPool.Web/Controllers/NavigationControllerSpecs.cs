using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;

using Rhino.Mocks;
using Machine.Specifications;

using ClubPool.ApplicationServices.Interfaces;

using ClubPool.Core;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Navigation.ViewModels;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_navigation_controller
  {
    protected static NavigationController controller;
    protected static IRoleService roleService;

    Establish context = () => {
      roleService = MockRepository.GenerateStub<IRoleService>();
      controller = new NavigationController(roleService);
      ControllerHelper.CreateMockControllerContext(controller);
    };

  }

  [Subject(typeof(NavigationController))]
  public class when_the_navigation_controller_is_asked_for_the_menu_when_user_is_not_logged_in : specification_for_navigation_controller
  {
    static ActionResult result;
    static IIdentity identity;

    Establish context = () => {
      identity = controller.HttpContext.User.Identity;
      identity.Expect(i => i.IsAuthenticated).Return(false);
    };

    Because of = () => result = controller.Menu();

    It should_ask_if_the_user_is_logged_in = () =>
      identity.AssertWasCalled(i => i.IsAuthenticated);

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
  public class when_the_navigation_controller_is_asked_for_the_menu_when_normal_user_is_logged_in : specification_for_navigation_controller
  {
    static ActionResult result;
    static IIdentity identity;

    private static string username = "TestUser";
    private static string rolename = Roles.Administrators;

    Establish context = () => {
      identity = controller.HttpContext.User.Identity;
      identity.Expect(i => i.IsAuthenticated).Return(true);
      identity.Expect(i => i.Name).Return(username);
      roleService.Expect(r => r.IsUserInRole(username, rolename)).Return(false);
    };

    Because of = () => result = controller.Menu();

    It should_ask_if_the_user_is_logged_in = () =>
      identity.AssertWasCalled(i => i.IsAuthenticated);

    It should_ask_if_the_user_is_in_the_Administrators_role = () =>
      roleService.AssertWasCalled(r => r.IsUserInRole(username, rolename));

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
  public class when_the_navigation_controller_is_asked_for_the_menu_when_admin_user_is_logged_in : specification_for_navigation_controller
  {
    static ActionResult result;
    static IIdentity identity;

    private static string username = "TestUser";
    private static string rolename = Roles.Administrators;

    Establish context = () => {
      identity = controller.HttpContext.User.Identity;
      identity.Expect(i => i.IsAuthenticated).Return(true);
      identity.Expect(i => i.Name).Return(username);
      roleService.Expect(r => r.IsUserInRole(username, rolename)).Return(true);
    };

    Because of = () => result = controller.Menu();

    It should_ask_if_the_user_is_logged_in = () =>
      identity.AssertWasCalled(i => i.IsAuthenticated);

    It should_ask_if_the_user_is_in_the_Administrators_role = () =>
      roleService.AssertWasCalled(r => r.IsUserInRole(username, rolename));

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
