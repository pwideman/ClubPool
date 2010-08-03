using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;

using Rhino.Mocks;
using Machine.Specifications;
using SharpArch.Testing;

using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Messaging.Contracts;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Users;
using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Framework.NHibernate;
using ClubPool.Testing.ApplicationServices.Authentication;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Users
{
  public abstract class specification_for_users_controller
  {
    protected static UsersController controller;
    protected static ILinqRepository<Role> roleRepository;
    protected static MockAuthenticationService authenticationService;
    protected static IMembershipService membershipService;
    protected static IUserRepository userRepository;
    protected static IEmailService emailService;

    Establish context = () => {
      roleRepository = MockRepository.GenerateStub<ILinqRepository<Role>>();
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      membershipService = MockRepository.GenerateStub<IMembershipService>();
      userRepository = MockRepository.GenerateStub<IUserRepository>();
      emailService = MockRepository.GenerateStub<IEmailService>();
      controller = new UsersController(authenticationService, membershipService, emailService, userRepository, roleRepository);
      ControllerHelper.CreateMockControllerContext(controller);
      ServiceLocatorHelper.AddValidator();
    };
  }

  // We don't need to test the paging here, that is tested in PagedListViewModelBaseSpecs
  [Subject(typeof(UsersController))]
  public class when_asked_for_the_default_view : specification_for_users_controller
  {
    static ActionResult result;
    static int page = 1;
    static int pages = 3;
    static int pageSize = 10;

    Establish context = () => {
      var users = new List<User>();
      for (var i = 0; i < pages * pageSize; i++) {
        users.Add(new User("user" + i.ToString(), "pass", "user", i.ToString(), "user" + i.ToString() + "@user.com"));
      }
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = controller.Index(page);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as IndexViewModel;
      viewModel.ShouldNotBeNull();
      viewModel.Items.Count().ShouldEqual(pageSize);
      viewModel.First.ShouldEqual((page-1) * pageSize + 1);
      viewModel.Last.ShouldEqual(pageSize * page);
      viewModel.Total.ShouldEqual(pageSize * pages);
      viewModel.TotalPages.ShouldEqual(pages);
      viewModel.CurrentPage.ShouldEqual(page);
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_view_and_the_user_is_not_logged_in : specification_for_users_controller
  {
    static ActionResult result;
    static string returnUrl;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = false;
      returnUrl = "some return url";
    };

    Because of = () => result = controller.Login(returnUrl);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as LoginViewModel;
      var viewResult = result as ViewResult;
      viewResult.TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage).ShouldBeFalse();
      viewModel.ReturnUrl.ShouldEqual(returnUrl);
      viewModel.Password.ShouldBeNull();
      viewModel.StayLoggedIn.ShouldBeFalse();
      viewModel.Username.ShouldBeNull();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_view_and_the_user_is_logged_in : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
    };

    Because of = () => result = controller.Login(string.Empty);

    It should_redirect_to_home_index = () => {
      result.IsARedirectToARouteAnd().ControllerName().ShouldEqual("Dashboard");
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Index");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_login_with_a_return_url_and_authentication_is_successful : specification_for_users_controller
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

    It should_redirect_to_the_return_url = () => 
      result.IsARedirectAnd().Url.ShouldEqual(returnUrl);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_login_without_a_return_url_and_authentication_is_successful : specification_for_users_controller
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

    It should_redirect_to_home_index = () => {
      result.IsARedirectToARouteAnd().ControllerName().ShouldEqual("Dashboard");
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Index");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_login_and_authentication_is_unsuccessful : specification_for_users_controller
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

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var vm = result.IsAViewAnd().ViewData.Model as LoginViewModel;
      var viewResult = result as ViewResult;
      vm.Username.ShouldEqual(username);
      vm.Password.ShouldBeEmpty();
      viewResult.TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();
      viewResult.TempData[GlobalViewDataProperty.PageErrorMessage].ShouldEqual("Invalid username/password");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_status_view_and_the_user_is_not_logged_in : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = false;
    };

    Because of = () => result = controller.LoginStatus();

    It should_return_the_default_view = () =>
      result.IsAPartialViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAPartialViewAnd().ViewData.Model as LoginStatusViewModel;
      viewModel.Username.ShouldBeNull();
      viewModel.UserIsLoggedIn.ShouldBeFalse();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_status_view_and_the_user_is_logged_in : specification_for_users_controller
  {
    static ActionResult result;
    static string username = "TestUser";

    Establish context = () => {
      var principal = authenticationService.MockPrincipal;
      principal.MockIdentity.Name = username;
      principal.MockIdentity.IsAuthenticated = true;
    };

    Because of = () => result = controller.LoginStatus();

    It should_return_the_default_view = () =>
      result.IsAPartialViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties_correctly = () => {
      var viewModel = result.IsAPartialViewAnd().ViewData.Model as LoginStatusViewModel;
      viewModel.Username.ShouldEqual(username);
      viewModel.UserIsLoggedIn.ShouldBeTrue();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_logout : specification_for_users_controller
  {
    static ActionResult result;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
    };

    Because of = () => result = controller.Logout();

    It should_log_the_user_out = () =>
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated.ShouldBeFalse();

    It should_redirect_to_home_index = () => {
      result.IsARedirectToARouteAnd().ControllerName().ShouldEqual("Home");
      result.IsARedirectToARouteAnd().ActionName().ShouldEqual("Index");
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_sidebar_gadget : specification_for_users_controller
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
  public class when_asked_for_the_signup_view : specification_for_users_controller
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
  public class when_asked_to_sign_up_a_new_user_and_the_viewModel_is_invalid : specification_for_users_controller
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
  public class when_asked_to_sign_up_a_new_user_and_password_and_confirm_password_do_not_match : specification_for_users_controller
  {
    static ActionResult result;
    static SignUpViewModel viewModel;

    Establish context = () => {
      viewModel = new SignUpViewModel();
      viewModel.Username = "test";
      viewModel.ConfirmPassword = "test";
      viewModel.Password = "Password";
      viewModel.Email = "test@test.com";
      viewModel.FirstName = "test";
      viewModel.LastName = "test";
    };

    Because of = () => result = controller.SignUp(viewModel, true);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_pass_the_view_model_back_to_the_view = () => {
      var vm = result.IsAViewAnd().ViewData.Model as SignUpViewModel;
      vm.ShouldNotBeNull();
      vm.Username.ShouldEqual(viewModel.Username);
    };

    It should_set_the_model_state_errors = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.IsValid.ShouldBeFalse();
      modelState.Count.ShouldBeGreaterThan(0);
      modelState.Keys.Contains("ConfirmPassword").ShouldBeTrue();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_with_duplicate_username : specification_for_users_controller
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
      var viewResult = result as ViewResult;
      viewResult.ViewData.ModelState.ContainsKey("Username").ShouldBeTrue();
      viewResult.ViewData.ModelState["Username"].Errors.Count.ShouldBeGreaterThan(0);
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_with_duplicate_email : specification_for_users_controller
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
      var viewResult = result as ViewResult;
      viewResult.ViewData.ModelState.ContainsKey("Email").ShouldBeTrue();
      viewResult.ViewData.ModelState["Email"].Errors.Count.ShouldBeGreaterThan(0);
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_with_valid_info : specification_for_users_controller
  {
    static ActionResult result;
    static SignUpViewModel viewModel;
    static User user;
    static string username = "TestUser";
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

      membershipService.Stub(s => s.CreateUser(username, viewModel.Password, viewModel.FirstName, 
        viewModel.LastName, viewModel.Email, false, false)).Return(user);

      var role = MockRepository.GenerateStub<Role>(Roles.Officers);
      user = new User("officer", "officer", "officer", "user", "officer@email.com");
      role.Stub(r => r.Users).Return(new User[1] { user });
      roleRepository.Stub(r => r.FindOne(null)).IgnoreArguments().Return(role);
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
  public class when_asked_to_sign_up_a_new_user_and_the_captcha_is_invalid : specification_for_users_controller
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
  public class when_asked_for_the_unapproved_users_and_there_are_none : specification_for_users_controller
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
  public class when_asked_for_the_unapproved_users_view : specification_for_users_controller
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
  public class when_asked_to_approve_users : specification_for_users_controller
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
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_delete_a_user : specification_for_users_controller
  {
    static ActionResult result;
    static int userId = 1;
    static User user;
    static int page = 5;

    Establish context = () => {
      user = new User("test", "test", "test", "test", "test");
      userRepository.Stub(r => r.Get(userId)).Return(user);
      userRepository.Expect(r => r.Delete(user));
    };

    Because of = () => result = controller.Delete(userId, page);

    It should_delete_the_user = () =>
      userRepository.VerifyAllExpectations();

    It should_redirect_to_the_users_index_view = () => {
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("users");
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      var pageRouteValue = new KeyValuePair<string, object>("page", page);
      result.IsARedirectToARouteAnd().RouteValues.ShouldContain(pageRouteValue);
    };

    It should_set_the_notification_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_delete_an_invalid_user : specification_for_users_controller
  {
    static ActionResult result;
    static int userId = 1;
    static int page = 5;

    Establish context = () => {
      userRepository.Stub(r => r.Get(userId)).Return(null);
    };

    Because of = () => result = controller.Delete(userId, 5);

    It should_not_delete_the_user = () =>
      userRepository.AssertWasNotCalled(r => r.Delete(null), x => x.IgnoreArguments());

    It should_redirect_to_the_users_index_view = () => {
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("users");
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      var pageRouteValue = new KeyValuePair<string, object>("page", page);
      result.IsARedirectToARouteAnd().RouteValues.ShouldContain(pageRouteValue);
    };

    It should_set_the_error_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_edit_a_user : specification_for_users_controller
  {
    static ActionResult result;
    static int userId = 1;
    static User user;
    static List<Role> roles;

    Establish context = () => {
      roles = new List<Role>();
      for(var i=0; i< 5; i++) {
        var role = new Role("role" + i.ToString());
        role.SetIdTo(i);
        roles.Add(role);
      }
      user = new User("user", "pass", "user", "one", "user@user.com");
      user.AddRole(roles[0]);
      user.AddRole(roles[1]);
      user.SetIdTo(userId);
      userRepository.Stub(r => r.Get(userId)).Return(user);
      roleRepository.Stub(r => r.GetAll()).Return(roles.AsQueryable());
    };

    Because of = () => result = controller.Edit(userId);

    It should_display_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as EditViewModel;
      viewModel.ShouldNotBeNull();
      viewModel.Id.ShouldEqual(user.Id);
      viewModel.FirstName.ShouldEqual(user.FirstName);
      viewModel.LastName.ShouldEqual(user.LastName);
      viewModel.IsApproved.ShouldEqual(user.IsApproved);
      viewModel.Email.ShouldEqual(user.Email);
      viewModel.IsLocked.ShouldEqual(user.IsLocked);
      viewModel.Username.ShouldEqual(user.Username);
      viewModel.Roles.Count().ShouldEqual(user.Roles.Count());
      viewModel.AvailableRoles.Count().ShouldEqual(roles.Count);
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_valid_data : specification_for_users_controller
  {
    static ActionResult result;
    static EditViewModel viewModel;
    static User user;
    static int userId;
    static List<Role> roles;
    static User savedUser;

    Establish context = () => {
      userId = 1;

      roles = new List<Role>();
      for (var i = 0; i < 5; i++) {
        var role = new Role("role" + i.ToString());
        role.SetIdTo(i);
        roles.Add(role);
        roleRepository.Stub(r => r.Get(i)).Return(role);
      }

      viewModel = new EditViewModel() {
        FirstName = "user",
        LastName = "test",
        Username = "testuser",
        Email = "user@user.com",
        IsLocked = false,
        IsApproved = true,
        Id = userId,
        Roles = new int[] { 0, 1 }
      };

      user = new User("temp", "pass", "temp", "temp", "temp@temp.com");
      user.SetIdTo(userId);

      roleRepository.Stub(r => r.GetAll()).Return(roles.AsQueryable());
      userRepository.Stub(r => r.Get(userId)).Return(user);
      userRepository.Stub(r => r.SaveOrUpdate(Arg<User>.Is.Anything)).Return(null).WhenCalled(m => savedUser = (User)m.Arguments[0]);
    };

    Because of = () => result = controller.Edit(viewModel);

    It should_redirect_to_the_default_view = () => {
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("users");
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
    };

    It should_set_the_page_notification_message = () => 
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_update_the_user_properties = () => {
      savedUser.Username.ShouldEqual(viewModel.Username);
      savedUser.FirstName.ShouldEqual(viewModel.FirstName);
      savedUser.LastName.ShouldEqual(viewModel.LastName);
      savedUser.Email.ShouldEqual(viewModel.Email);
      savedUser.IsApproved.ShouldEqual(viewModel.IsApproved);
      savedUser.IsLocked.ShouldEqual(viewModel.IsLocked);
      savedUser.Id.ShouldEqual(viewModel.Id);
      savedUser.Roles.Count().ShouldEqual(viewModel.Roles.Count());
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_a_model_state_error : specification_for_users_controller
  {
    static ActionResult result;
    static EditViewModel viewModel;
    static int userId;

    Establish context = () => {
      userId = 1;
      viewModel = new EditViewModel() {
        FirstName = "user",
        LastName = "test",
        Username = null,
        Email = "user@user.com",
        IsLocked = false,
        IsApproved = true,
        Id = userId,
        Roles = new int[] { 0, 1 }
      };
    };

    Because of = () => result = controller.Edit(viewModel);

    It should_return_the_viewmodel_to_the_default_view = () => {
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
      result.IsAViewAnd().ViewData.Model.ShouldEqual(viewModel);
    };

    It should_add_the_model_state_error = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.IsValid.ShouldBeFalse();
      modelState.Count.ShouldBeGreaterThan(0);
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_a_duplicate_username : specification_for_users_controller
  {
    static ActionResult result;
    static EditViewModel viewModel;
    static int userId;
    static User user;

    Establish context = () => {
      userId = 1;
      viewModel = new EditViewModel() {
        FirstName = "user",
        LastName = "test",
        Username = "username",
        Email = "user@user.com",
        IsLocked = false,
        IsApproved = true,
        Id = userId,
        Roles = new int[] { 0, 1 }
      };
      
      user = new User("temp", "pass", "temp", "temp", "temp@temp.com");
      user.SetIdTo(userId);
      userRepository.Stub(r => r.Get(userId)).Return(user);
      userRepository.Stub(r => r.DbContext).Return(MockRepository.GenerateStub<SharpArch.Core.PersistenceSupport.IDbContext>());

      membershipService.Stub(s => s.UsernameIsInUse(null)).IgnoreArguments().Return(true);

      roleRepository.Stub(s => s.GetAll()).Return(new List<Role>().AsQueryable());
    };

    Because of = () => result = controller.Edit(viewModel);

    It should_return_the_viewmodel_to_the_default_view = () => {
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
      result.IsAViewAnd().ViewData.Model.ShouldEqual(viewModel);
    };

    It should_add_the_model_state_error = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.IsValid.ShouldBeFalse();
      modelState.Count.ShouldBeGreaterThan(0);
      modelState.Keys.Contains("Username").ShouldBeTrue();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_a_duplicate_email : specification_for_users_controller
  {
    static ActionResult result;
    static EditViewModel viewModel;
    static int userId;
    static User user;

    Establish context = () => {
      userId = 1;
      viewModel = new EditViewModel() {
        FirstName = "user",
        LastName = "test",
        Username = "username",
        Email = "user@user.com",
        IsLocked = false,
        IsApproved = true,
        Id = userId,
        Roles = new int[] { 0, 1 }
      };

      user = new User("temp", "pass", "temp", "temp", "temp@temp.com");
      user.SetIdTo(userId);
      userRepository.Stub(r => r.Get(userId)).Return(user);
      userRepository.Stub(r => r.DbContext).Return(MockRepository.GenerateStub<SharpArch.Core.PersistenceSupport.IDbContext>());
      membershipService.Stub(s => s.EmailIsInUse(null)).IgnoreArguments().Return(true);

      roleRepository.Stub(s => s.GetAll()).Return(new List<Role>().AsQueryable());

    };

    Because of = () => result = controller.Edit(viewModel);

    It should_return_the_viewmodel_to_the_default_view = () => {
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
      result.IsAViewAnd().ViewData.Model.ShouldEqual(viewModel);
    };

    It should_add_the_model_state_error = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.IsValid.ShouldBeFalse();
      modelState.Count.ShouldBeGreaterThan(0);
      modelState.Keys.Contains("Email").ShouldBeTrue();
    };
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_create_view : specification_for_users_controller
  {
    static ActionResult result;

    Because of = () => result = controller.Create();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
  }

  // we don't have to test much with Create because it's the same code as SignUp.
  // technically our specs shouldn't know this and exercise them both, but...
  [Subject(typeof(UsersController))]
  public class when_asked_to_create_a_user : specification_for_users_controller
  {
    static ActionResult result;
    static User user;
    static CreateViewModel viewModel;

    Establish context = () => {
      viewModel = new CreateViewModel();
      viewModel.ConfirmPassword = "pass";
      viewModel.Password = "pass";
      viewModel.Username = "user";
      viewModel.FirstName = "user";
      viewModel.LastName = "user";
      viewModel.Email = "user@user.com";

      membershipService.Stub(s => s.CreateUser(null, null, null, null, null, false, false)).IgnoreArguments().WhenCalled(m => {
        user = new User(m.Arguments[0] as string, m.Arguments[1] as string, m.Arguments[2] as string,
          m.Arguments[3] as string, m.Arguments[4] as string);
        user.IsApproved = (bool)m.Arguments[5];
        user.IsLocked = (bool)m.Arguments[6];
      });
    };

    Because of = () => result = controller.Create(viewModel);

    It should_create_the_user_approved_and_unlocked = () => {
      user.Username.ShouldEqual(viewModel.Username);
      user.FirstName.ShouldEqual(viewModel.FirstName);
      user.LastName.ShouldEqual(viewModel.LastName);
      // etc.
      user.IsLocked.ShouldBeFalse();
      user.IsApproved.ShouldBeTrue();
    };
  }
}
