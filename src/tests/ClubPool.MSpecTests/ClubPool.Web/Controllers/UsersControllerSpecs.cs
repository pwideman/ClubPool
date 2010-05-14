﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;
using System.Web.Security;

using Rhino.Mocks;
using Machine.Specifications;
using SharpArch.Testing;

using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Messaging.Contracts;
using ClubPool.Core;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Framework.NHibernate;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_users_controller
  {
    protected static UsersController controller;
    protected static IRoleService roleService;
    protected static IAuthenticationService authenticationService;
    protected static IMembershipService membershipService;
    protected static ILinqRepository<User> userRepository;
    protected static IEmailService emailService;

    Establish context = () => {
      roleService = MockRepository.GenerateStub<IRoleService>();
      authenticationService = MockRepository.GenerateStub<IAuthenticationService>();
      membershipService = MockRepository.GenerateStub<IMembershipService>();
      userRepository = MockRepository.GenerateStub<ILinqRepository<User>>();
      emailService = MockRepository.GenerateStub<IEmailService>();
      controller = new UsersController(authenticationService, membershipService, roleService, emailService, userRepository);
      ControllerHelper.CreateMockControllerContext(controller);
      ServiceLocatorHelper.AddValidator();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_for_the_default_view_and_user_is_not_logged_in : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => {
      authenticationService.Stub(svc => svc.IsLoggedIn()).Return(false);
    };

    Because of = () => result = controller.Index();

    It should_ask_if_the_user_is_logged_in = () =>
      authenticationService.AssertWasCalled(svc => svc.IsLoggedIn());

    It should_redirect_to_the_login_action = () =>
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Login");
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_for_the_default_view_and_user_is_logged_in : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => {
      authenticationService.Stub(svc => svc.IsLoggedIn()).Return(true);
    };

    Because of = () => result = controller.Index();

    It should_ask_if_the_user_is_logged_in = () =>
      authenticationService.AssertWasCalled(svc => svc.IsLoggedIn());

    It should_redirect_to_the_login_action = () => {
      result.IsARedirectToARouteAnd().ControllerName().ShouldEqual("Home");
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Index");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_for_the_login_view_and_the_user_is_not_logged_in : specification_for_users_controller
  {
    static ActionResult result;
    static string returnUrl;

    Establish context = () => {
      authenticationService.Stub(s => s.IsLoggedIn()).Return(false);
      returnUrl = "some return url";
    };

    Because of = () => result = controller.Login(returnUrl);

    It should_ask_the_authentication_service_if_the_user_is_logged_in = () =>
      authenticationService.AssertWasCalled(s => s.IsLoggedIn());

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as LoginViewModel;
      viewModel.ReturnUrl.ShouldEqual(returnUrl);
      viewModel.ErrorMessage.ShouldBeNull();
      viewModel.Password.ShouldBeNull();
      viewModel.StayLoggedIn.ShouldBeFalse();
      viewModel.Username.ShouldBeNull();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_for_the_login_view_and_the_user_is_logged_in : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => {
      authenticationService.Stub(s => s.IsLoggedIn()).Return(true);
    };

    Because of = () => result = controller.Login(string.Empty);

    It should_ask_the_authentication_service_if_the_user_is_logged_in = () =>
      authenticationService.AssertWasCalled(s => s.IsLoggedIn());

    It should_redirect_to_home_index = () => {
      result.IsARedirectToARouteAnd().ControllerName().ShouldEqual("Home");
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Index");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_to_login_with_a_return_url_and_authentication_is_successful : specification_for_users_controller
  {
    static ActionResult result;
    static string username = "TestUser";
    static string password = "TestPassword";
    static bool stayLoggedIn = true;
    static string returnUrl = "some url";
    static LoginViewModel viewModel;

    Establish context = () => {
      membershipService.Stub(s => s.ValidateUser(username, password)).Return(true);
      viewModel = new LoginViewModel { 
        Username = username, 
        Password = password, 
        StayLoggedIn = stayLoggedIn,
        ReturnUrl = returnUrl
      };
    };

    Because of = () => result = controller.Login(viewModel);

    It should_ask_the_membership_service_to_validate_the_user = () =>
      membershipService.AssertWasCalled(s => s.ValidateUser(username, password));

    It should_ask_the_authentication_service_to_log_in = () =>
      authenticationService.AssertWasCalled(s => s.LogIn(username, stayLoggedIn));

    It should_redirect_to_the_return_url = () => 
      result.IsARedirectAnd().Url.ShouldEqual(returnUrl);
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_to_login_without_a_return_url_and_authentication_is_successful : specification_for_users_controller
  {
    static ActionResult result;
    static string username = "TestUser";
    static string password = "TestPassword";
    static bool stayLoggedIn = true;
    static LoginViewModel viewModel;

    Establish context = () => {
      membershipService.Stub(s => s.ValidateUser(username, password)).Return(true);
      viewModel = new LoginViewModel {
        Username = username,
        Password = password,
        StayLoggedIn = stayLoggedIn
      };
    };

    Because of = () => result = controller.Login(viewModel);

    It should_ask_the_membership_service_to_validate_the_user = () =>
      membershipService.AssertWasCalled(s => s.ValidateUser(username, password));

    It should_ask_the_authentication_service_to_log_in = () =>
      authenticationService.AssertWasCalled(s => s.LogIn(username, stayLoggedIn));

    It should_redirect_to_home_index = () => {
      result.IsARedirectToARouteAnd().ControllerName().ShouldEqual("Dashboard");
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Index");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_to_login_and_authentication_is_unsuccessful : specification_for_users_controller
  {
    static ActionResult result;
    static string username = "TestUser";
    static string password = "TestPassword";
    static LoginViewModel viewModel;

    Establish context = () => {
      membershipService.Stub(s => s.ValidateUser(username, password)).Return(false);
      viewModel = new LoginViewModel {
        Username = username,
        Password = password,
      };
    };

    Because of = () => result = controller.Login(viewModel);

    It should_ask_the_membership_service_to_validate_the_user = () =>
      membershipService.AssertWasCalled(s => s.ValidateUser(username, password));

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var vm = result.IsAViewAnd().ViewData.Model as LoginViewModel;
      vm.Username.ShouldEqual(username);
      vm.Password.ShouldBeEmpty();
      vm.ErrorMessage.ShouldEqual("Invalid username/password");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_for_the_login_status_view_and_the_user_is_not_logged_in : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => {
      authenticationService.Stub(s => s.IsLoggedIn()).Return(false);
    };

    Because of = () => result = controller.LoginStatus();

    It should_ask_the_authentication_service_if_the_user_is_logged_in = () =>
      authenticationService.AssertWasCalled(s => s.IsLoggedIn());

    It should_return_the_default_view = () =>
      result.IsAPartialViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAPartialViewAnd().ViewData.Model as LoginStatusViewModel;
      viewModel.Username.ShouldBeNull();
      viewModel.UserIsLoggedIn.ShouldBeFalse();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_for_the_login_status_view_and_the_user_is_logged_in : specification_for_users_controller
  {
    static ActionResult result;
    static string username = "TestUser";

    Establish context = () => {
      authenticationService.Stub(s => s.IsLoggedIn()).Return(true);
      var identity = new Identity { Username = username };
      authenticationService.Stub(s => s.GetCurrentIdentity()).Return(identity);
    };

    Because of = () => result = controller.LoginStatus();

    It should_ask_the_authentication_service_if_the_user_is_logged_in = () =>
      authenticationService.AssertWasCalled(s => s.IsLoggedIn());

    It should_return_the_default_view = () =>
      result.IsAPartialViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAPartialViewAnd().ViewData.Model as LoginStatusViewModel;
      viewModel.Username.ShouldEqual(username);
      viewModel.UserIsLoggedIn.ShouldBeTrue();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_to_logout : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => { };

    Because of = () => result = controller.Logout();

    It should_ask_the_authentication_service_to_log_out = () =>
      authenticationService.AssertWasCalled(s => s.LogOut());

    It should_redirect_to_home_index = () => {
      result.IsARedirectToARouteAnd().ControllerName().ShouldEqual("Home");
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Index");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_user_controller_is_asked_for_the_login_sidebar_gadget : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => { };

    Because of = () => result = controller.LoginGadget();

    It should_return_the_default_view = () =>
      result.IsAPartialViewAnd().ViewName.ShouldBeEmpty();

    It should_pass_a_login_view_model_object_to_the_view = () =>
      (result.IsAPartialViewAnd().ViewData.Model is LoginViewModel).ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_for_the_signup_view : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => { };

    Because of = () => result = controller.SignUp();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_pass_a_signup_view_model_object_to_the_view = () =>
      (result.IsAViewAnd().ViewData.Model is SignUpViewModel).ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_to_sign_up_a_new_user_and_the_viewModel_is_invalid : specification_for_users_controller
  {
    static ActionResult result;
    static SignUpViewModel viewModel;
    static string username = "TestUser";

    Establish context = () => {
      viewModel = new SignUpViewModel();
      viewModel.Username = username;
    };

    Because of = () => result = controller.SignUp(viewModel, true);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_pass_the_view_model_back_to_the_view = () => {
      var vm = result.IsAViewAnd().ViewData.Model as SignUpViewModel;
      vm.ShouldNotBeNull();
      vm.Username.ShouldEqual(username);
    };

    It should_set_the_model_state_errors = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.IsValid.ShouldBeFalse();
      modelState.Count.ShouldBeGreaterThan(0);
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_to_sign_up_a_new_user_with_duplicate_username : specification_for_users_controller
  {
    static ActionResult result;
    static SignUpViewModel viewModel;
    static string username = "TestUser";

    Establish context = () => {
      viewModel = new SignUpViewModel();
      viewModel.Username = username;
      viewModel.Password = "test";
      viewModel.ConfirmPassword = "test";
      viewModel.Email = "test@test.com";
      viewModel.FirstName = "test";
      viewModel.LastName = "test";

      membershipService.Expect(s => s.UsernameIsInUse(null)).IgnoreArguments().Return(true);
    };

    Because of = () => result = controller.SignUp(viewModel, true);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_pass_the_view_model_back_to_the_view_with_error_message = () => {
      var vm = result.IsAViewAnd().ViewData.Model as SignUpViewModel;
      vm.ShouldNotBeNull();
      vm.Username.ShouldEqual(username);
      vm.ErrorMessage.ShouldNotBeEmpty();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_to_sign_up_a_new_user_with_duplicate_email : specification_for_users_controller
  {
    static ActionResult result;
    static SignUpViewModel viewModel;
    static string email = "TestEmail@email.com";

    Establish context = () => {
      viewModel = new SignUpViewModel();
      viewModel.Username = "test";
      viewModel.Password = "test";
      viewModel.ConfirmPassword = "test";
      viewModel.Email = email;
      viewModel.FirstName = "test";
      viewModel.LastName = "test";

      membershipService.Expect(s => s.EmailIsInUse(null)).IgnoreArguments().Return(true);
    };

    Because of = () => result = controller.SignUp(viewModel, true);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_pass_the_view_model_back_to_the_view_with_error_message = () => {
      var vm = result.IsAViewAnd().ViewData.Model as SignUpViewModel;
      vm.ShouldNotBeNull();
      vm.Email.ShouldEqual(email);
      vm.ErrorMessage.ShouldNotBeEmpty();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_to_sign_up_a_new_user_with_valid_info : specification_for_users_controller
  {
    static ActionResult result;
    static SignUpViewModel viewModel;
    static User user;
    static string username = "TestUser";
    static string adminUsername = "admin";
    static string adminEmail = "admin@email.com";
    static string userEmail = "test@test.com";

    Establish context = () => {
      viewModel = new SignUpViewModel();
      viewModel.Username = username;
      viewModel.Password = "test";
      viewModel.ConfirmPassword = "test";
      viewModel.Email = userEmail;
      viewModel.FirstName = "test";
      viewModel.LastName = "test";

      user = new User(viewModel.Username, viewModel.Password, viewModel.FirstName, viewModel.LastName, viewModel.Email);

      membershipService.Stub(s => s.CreateUser(username, viewModel.Password, viewModel.FirstName, viewModel.LastName, viewModel.Email, false)).Return(user);

      roleService.Stub(s => s.GetUsersInRole(Core.Roles.Administrators))
        .Return(new string[] { adminUsername });

      userRepository.Stub(r => r.GetAll()).Return(
        new List<User>() { 
          new User(adminUsername, adminUsername, adminUsername, adminUsername, adminEmail)
        }.AsQueryable());
    };

    Because of = () => result = controller.SignUp(viewModel, true);

    It should_return_the_SignUpComplete_view = () =>
      result.IsAViewAnd().ViewName.ShouldEqual("SignUpComplete");

    It should_send_new_user_awaiting_approval_email_to_all_admins = () => {
      emailService.AssertWasCalled(s => s.SendSystemEmail(new List<string>(), null, null),
        o => o.IgnoreArguments());
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_to_sign_up_a_new_user_and_the_captcha_is_invalid : specification_for_users_controller
  {
    static ActionResult result;
    static SignUpViewModel viewModel;
    static string username = "TestUser";

    Establish context = () => {
      viewModel = new SignUpViewModel();
      viewModel.Username = username;
    };

    Because of = () => result = controller.SignUp(viewModel, false);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_pass_the_view_model_back_to_the_view = () => {
      var vm = result.IsAViewAnd().ViewData.Model as SignUpViewModel;
      vm.ShouldNotBeNull();
      vm.Username.ShouldEqual(username);
    };

    It should_set_the_model_state_errors = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.IsValid.ShouldBeFalse();
      modelState.Count.ShouldBeGreaterThan(0);
      modelState.Keys.Contains("captcha").ShouldBeTrue();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_for_the_unapproved_users_and_there_are_none : specification_for_users_controller
  {
    static ActionResult result;
    static IList<User> users;

    Establish context = () => {
      users = new List<User>();
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = controller.Unapproved();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_not_display_any_unapproved_users = () => {
      var vm = result.IsAViewAnd().ViewData.Model as UnapprovedViewModel;
      vm.ShouldNotBeNull();
      vm.UnapprovedUsers.Count().ShouldEqual(0);
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_for_the_unapproved_users_view : specification_for_users_controller
  {
    static ActionResult result;
    static IList<User> users;

    Establish context = () => {
      users = new List<User>() {
        new User("user1", "user1", "user", "one", "user@user.com"),
        new User("user2", "user2", "user", "two", "user2@user.com"),
        new User("user3", "user3", "user", "three", "user3@user.com") { IsApproved = true }
      };
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = controller.Unapproved();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_pass_the_view_model_to_the_view = () => {
      var vm = result.IsAViewAnd().ViewData.Model as UnapprovedViewModel;
      vm.ShouldNotBeNull();
      vm.UnapprovedUsers.Count().ShouldEqual(2);
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_user_controller_is_asked_to_approve_users : specification_for_users_controller
  {
    static ActionResult result;
    static IList<User> users;

    Establish context = () => {
      users = new List<User>() {
        new User("user1", "user1", "user", "one", "user@user.com").SetIdTo(1) as User,
        new User("user2", "user2", "user", "two", "user2@user.com").SetIdTo(2) as User,
        new User("user3", "user3", "user", "three", "user3@user.com").SetIdTo(3) as User
      };
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = controller.Approve(new int[] {1,2});

    It should_set_the_IsApproved_property_on_the_approved_user = () => {
      users[0].IsApproved.ShouldBeTrue();
      users[1].IsApproved.ShouldBeTrue();
      users[2].IsApproved.ShouldBeFalse();
    };

    It should_redirect_to_the_unapproved_view = () => {
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("users");
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("unapproved");
    };

    It should_set_the_tempdata_message = () =>
      controller.TempData.ContainsKey("message").ShouldBeTrue();
  }
}