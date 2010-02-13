using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;

using Rhino.Mocks;
using Machine.Specifications;

using ClubPool.ApplicationServices.Contracts;

using ClubPool.Core;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.User.ViewModels;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_user_controller
  {
    protected static UserController controller;
    protected static IRoleService roleService;
    protected static IAuthenticationService authenticationService;
    protected static IMembershipService membershipService;

    Establish context = () => {
      roleService = MockRepository.GenerateStub<IRoleService>();
      authenticationService = MockRepository.GenerateStub<IAuthenticationService>();
      membershipService = MockRepository.GenerateStub<IMembershipService>();
      controller = new UserController(authenticationService, membershipService, roleService);
      ControllerHelper.CreateMockControllerContext(controller);
    };
  }

  [Subject(typeof(UserController))]
  public class when_user_controller_is_asked_for_the_default_view_and_user_is_not_logged_in : specification_for_user_controller
  {
    static ActionResult result;

    Establish context = () => {
      authenticationService.Expect(svc => svc.IsLoggedIn()).Return(false);
    };

    Because of = () => result = controller.Index();

    It should_ask_if_the_user_is_logged_in = () =>
      authenticationService.AssertWasCalled(svc => svc.IsLoggedIn());

    It should_redirect_to_the_login_action = () =>
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Login");
  }
}
