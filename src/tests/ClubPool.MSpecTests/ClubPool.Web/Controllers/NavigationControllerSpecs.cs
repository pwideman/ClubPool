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
    static PartialViewResultHelper<MenuViewModel> resultHelper;

    Establish context = () => {
      authService.MockPrincipal.MockIdentity.IsAuthenticated = false;
    };

    Because of = () => resultHelper = new PartialViewResultHelper<MenuViewModel>(controller.Menu());

    It should_indicate_that_the_user_is_not_logged_in = () =>
      resultHelper.Model.UserIsLoggedIn.ShouldBeFalse();

    It should_indicate_that_the_admin_menu_should_not_be_displayed = () =>
      resultHelper.Model.DisplayAdminMenu.ShouldBeFalse();
  }

  [Subject(typeof(NavigationController))]
  public class when_asked_for_the_menu_when_normal_user_is_logged_in : specification_for_navigation_controller
  {
    static PartialViewResultHelper<MenuViewModel> resultHelper;

    Establish context = () => {
      var principal = authService.MockPrincipal;
      principal.MockIdentity.IsAuthenticated = true;
    };

    Because of = () => resultHelper = new PartialViewResultHelper<MenuViewModel>(controller.Menu());

    It should_indicate_that_the_user_is_logged_in = () =>
      resultHelper.Model.UserIsLoggedIn.ShouldBeTrue();

    It should_indicate_that_the_admin_menu_should_not_be_displayed = () =>
      resultHelper.Model.DisplayAdminMenu.ShouldBeFalse();
  }

  [Subject(typeof(NavigationController))]
  public class when_asked_for_the_menu_when_admin_user_is_logged_in : specification_for_navigation_controller
  {
    static PartialViewResultHelper<MenuViewModel> resultHelper;

    Establish context = () => {
      var principal = authService.MockPrincipal;
      principal.MockIdentity.IsAuthenticated = true;
      principal.Roles = new string[] { Roles.Administrators };
    };

    Because of = () => resultHelper = new PartialViewResultHelper<MenuViewModel>(controller.Menu());

    It should_indicate_that_the_user_is_logged_in = () =>
      resultHelper.Model.UserIsLoggedIn.ShouldBeTrue();

    It should_indicate_that_the_admin_menu_should_be_displayed = () =>
      resultHelper.Model.DisplayAdminMenu.ShouldBeTrue();
  }
}
