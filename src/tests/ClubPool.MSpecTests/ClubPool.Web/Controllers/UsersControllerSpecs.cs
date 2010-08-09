using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;
using System.Net;
using System.Web;

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
using ClubPool.Framework.Extensions;
using ClubPool.Testing.ApplicationServices.Authentication;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Users
{
  public abstract class specification_for_users_controller
  {
    protected static UsersController controller;
    protected static IRoleRepository roleRepository;
    protected static MockAuthenticationService authenticationService;
    protected static IMembershipService membershipService;
    protected static IUserRepository userRepository;
    protected static IEmailService emailService;

    Establish context = () => {
      roleRepository = MockRepository.GenerateStub<IRoleRepository>();
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
    static ViewResultHelper<IndexViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index(page));

    It should_set_the_number_of_users_to_the_page_size = () =>
      resultHelper.Model.Items.Count().ShouldEqual(pageSize);

    It should_set_the_first_user_index = () =>
      resultHelper.Model.First.ShouldEqual((page - 1) * pageSize + 1);

    It should_set_the_last_user_index = () =>
      resultHelper.Model.Last.ShouldEqual(pageSize * page);

    It should_set_the_current_page_index = () =>
      resultHelper.Model.CurrentPage.ShouldEqual(page);

    It should_set_the_total_number_of_users = () =>
      resultHelper.Model.Total.ShouldEqual(pageSize * pages);

    It should_set_the_total_pages = () =>
      resultHelper.Model.TotalPages.ShouldEqual(pages);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_view_and_the_user_is_not_logged_in : specification_for_users_controller
  {
    static ViewResultHelper<LoginViewModel> resultHelper;
    static string returnUrl;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = false;
      returnUrl = "some return url";
    };

    Because of = () => resultHelper = new ViewResultHelper<LoginViewModel>(controller.Login(returnUrl));

    It should_set_the_return_url = () =>
      resultHelper.Model.ReturnUrl.ShouldEqual(returnUrl);

    It should_set_the_password_to_empty = () =>
      resultHelper.Model.Password.ShouldBeNull();

    It should_set_the_username_to_empty = () =>
      resultHelper.Model.Username.ShouldBeNull();

    It should_set_StayLoggedIn_to_false = () =>
      resultHelper.Model.StayLoggedIn.ShouldBeFalse();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_view_and_the_user_is_logged_in : specification_for_users_controller
  {
    static RedirectToRouteResultHelper resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Login(string.Empty));

    It should_redirect_to_the_dashboard_view = () =>
      resultHelper.ShouldRedirectTo("dashboard");
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_login_with_a_return_url_and_authentication_is_successful : specification_for_users_controller
  {
    static RedirectResultHelper resultHelper;
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

    Because of = () => resultHelper = new RedirectResultHelper(controller.Login(viewModel));

    It should_redirect_to_the_return_url = () => 
      resultHelper.Result.Url.ShouldEqual(returnUrl);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_login_without_a_return_url_and_authentication_is_successful : specification_for_users_controller
  {
    static RedirectToRouteResultHelper resultHelper;
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

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Login(viewModel));

    It should_redirect_to_the_dashboard_view = () =>
      resultHelper.ShouldRedirectTo("dashboard");
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_login_and_authentication_is_unsuccessful : specification_for_users_controller
  {
    static ViewResultHelper<LoginViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<LoginViewModel>(controller.Login(viewModel));

    It should_set_the_username_to_the_value_provided_by_the_user = () =>
      resultHelper.Model.Username.ShouldEqual(username);

    It should_set_the_password_to_empty = () =>
      resultHelper.Model.Password.ShouldBeEmpty();

    It should_set_the_page_error_message = () =>
      resultHelper.Result.TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_status_view_and_the_user_is_not_logged_in : specification_for_users_controller
  {
    static PartialViewResultHelper<LoginStatusViewModel> resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = false;
    };

    Because of = () => resultHelper = new PartialViewResultHelper<LoginStatusViewModel>(controller.LoginStatus());

    It should_set_the_username_to_empty = () =>
      resultHelper.Model.Username.ShouldBeNull();

    It should_indicate_that_the_user_is_not_logged_in = () =>
      resultHelper.Model.UserIsLoggedIn.ShouldBeFalse();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_status_view_and_the_user_is_logged_in : specification_for_users_controller
  {
    static PartialViewResultHelper<LoginStatusViewModel> resultHelper;
    static string username = "TestUser";

    Establish context = () => {
      var principal = authenticationService.MockPrincipal;
      principal.MockIdentity.Name = username;
      principal.MockIdentity.IsAuthenticated = true;
    };

    Because of = () => resultHelper = new PartialViewResultHelper<LoginStatusViewModel>(controller.LoginStatus());

    It should_set_the_username_to_the_username_of_the_user = () =>
      resultHelper.Model.Username.ShouldEqual(username);

    It should_indicate_that_the_user_is_logged_in = () =>
      resultHelper.Model.UserIsLoggedIn.ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_logout : specification_for_users_controller
  {
    static RedirectToRouteResultHelper resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Logout());

    It should_log_the_user_out = () =>
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated.ShouldBeFalse();

    It should_redirect_to_the_home_view = () =>
      resultHelper.ShouldRedirectTo("home");
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_login_sidebar_gadget : specification_for_users_controller
  {
    static PartialViewResultHelper<LoginViewModel> resultHelper;

    Establish context = () => { };

    Because of = () => resultHelper = new PartialViewResultHelper<LoginViewModel>(controller.LoginGadget());

    It should_set_the_username_to_empty = () =>
      resultHelper.Model.Username.ShouldBeEmpty();

    It should_set_the_password_to_empty = () =>
      resultHelper.Model.Password.ShouldBeEmpty();

    It should_set_the_return_url_to_empty = () =>
      resultHelper.Model.ReturnUrl.ShouldBeEmpty();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_and_the_view_model_is_invalid : specification_for_users_controller
  {
    static ViewResultHelper<SignUpViewModel> resultHelper;
    static SignUpViewModel viewModel;
    static string username = "TestUser";

    Establish context = () => {
      viewModel = new SignUpViewModel();
      viewModel.Username = username;
    };

    Because of = () => resultHelper = new ViewResultHelper<SignUpViewModel>(controller.SignUp(viewModel, true));

    It should_pass_the_data_provided_by_the_user_back_to_the_view = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_indicate_that_there_was_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_and_password_and_confirm_password_do_not_match : specification_for_users_controller
  {
    static ViewResultHelper<SignUpViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<SignUpViewModel>(controller.SignUp(viewModel, true));

    It should_pass_the_data_provided_by_the_user_back_to_the_view = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_indicate_that_there_was_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_that_the_error_was_related_to_the_ConfirmPassword_field = () =>
      resultHelper.Result.ViewData.ModelState.Keys.Contains("ConfirmPassword").ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_with_duplicate_username : specification_for_users_controller
  {
    static ViewResultHelper<SignUpViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<SignUpViewModel>(controller.SignUp(viewModel, true));

    It should_indicate_that_there_was_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_that_the_error_was_related_to_the_Username_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Username").ShouldBeTrue();

    It should_retain_the_data_that_the_user_has_already_entered = () =>
      resultHelper.Model.ShouldEqual(viewModel);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_with_duplicate_email : specification_for_users_controller
  {
    static ViewResultHelper<SignUpViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<SignUpViewModel>(controller.SignUp(viewModel, true));

    It should_indicate_that_there_was_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_that_the_error_was_related_to_the_Email_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Email").ShouldBeTrue();

    It should_retain_the_data_that_the_user_has_already_entered = () =>
      resultHelper.Model.ShouldEqual(viewModel);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_with_valid_info : specification_for_users_controller
  {
    static ViewResultHelper<object> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<object>(controller.SignUp(viewModel, true));

    It should_return_the_SignUpComplete_view = () =>
      resultHelper.Result.ViewName.ShouldEqual("SignUpComplete");

    It should_send_new_user_awaiting_approval_email_to_all_admins = () =>
      emailService.AssertWasCalled(s => s.SendSystemEmail(new List<string>(), null, null),
        o => o.IgnoreArguments());
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_sign_up_a_new_user_and_the_captcha_is_invalid : specification_for_users_controller
  {
    static ViewResultHelper<SignUpViewModel> resultHelper;
    static SignUpViewModel viewModel;
    static string username = "TestUser";

    Establish context = () => {
      viewModel = new SignUpViewModel();
      viewModel.Username = username;
    };

    Because of = () => resultHelper = new ViewResultHelper<SignUpViewModel>(controller.SignUp(viewModel, false));

    It should_indicate_that_there_was_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_that_the_error_was_related_to_the_captcha_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("captcha").ShouldBeTrue();

    It should_retain_the_data_that_the_user_has_already_entered = () =>
      resultHelper.Model.ShouldEqual(viewModel);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_unapproved_users_and_there_are_none : specification_for_users_controller
  {
    static ViewResultHelper<UnapprovedViewModel> resultHelper;
    static IList<User> users;

    Establish context = () => {
      users = new List<User>();
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => resultHelper = new ViewResultHelper<UnapprovedViewModel>(controller.Unapproved());

    It should_not_return_any_unapproved_users = () =>
      resultHelper.Model.UnapprovedUsers.Count().ShouldEqual(0);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_unapproved_users_view : specification_for_users_controller
  {
    static ViewResultHelper<UnapprovedViewModel> resultHelper;
    static IList<User> users;

    Establish context = () => {
      users = new List<User>() {
        new User("user1", "user1", "user", "one", "user@user.com"),
        new User("user2", "user2", "user", "two", "user2@user.com"),
        new User("user3", "user3", "user", "three", "user3@user.com") { IsApproved = true }
      };
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => resultHelper = new ViewResultHelper<UnapprovedViewModel>(controller.Unapproved());

    It should_return_the_unapproved_users = () =>
      users.Where(u => !u.IsApproved).Each(u => resultHelper.Model.UnapprovedUsers.ShouldContain(new UnapprovedUser(u)));
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_approve_users : specification_for_users_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static IList<User> users;

    Establish context = () => {
      users = new List<User>() {
        new User("user1", "user1", "user", "one", "user@user.com").SetIdTo(1) as User,
        new User("user2", "user2", "user", "two", "user2@user.com").SetIdTo(2) as User,
        new User("user3", "user3", "user", "three", "user3@user.com").SetIdTo(3) as User
      };
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Approve(new int[] {1,2}));

    It should_approve_the_selected_users = () => {
      users[0].IsApproved.ShouldBeTrue();
      users[1].IsApproved.ShouldBeTrue();
    };

    It should_not_approve_the_unselected_users = () =>
      users[2].IsApproved.ShouldBeFalse();

    It should_redirect_to_the_unapproved_users_view = () =>
      resultHelper.ShouldRedirectTo("users", "unapproved");

    It should_return_a_notification_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_delete_a_user : specification_for_users_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int userId = 1;
    static User user;
    static int page = 5;
    static KeyValuePair<string, object> pageRouteValue;

    Establish context = () => {
      user = new User("test", "test", "test", "test", "test");
      userRepository.Stub(r => r.Get(userId)).Return(user);
      userRepository.Expect(r => r.Delete(user));
      pageRouteValue = new KeyValuePair<string, object>("page", page);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Delete(userId, page));

    It should_delete_the_user = () =>
      userRepository.VerifyAllExpectations();

    It should_redirect_to_the_users__view = () =>
      resultHelper.ShouldRedirectTo("users");

    It should_retain_the_current_page = () =>
      resultHelper.Result.RouteValues.ShouldContain(pageRouteValue);

    It should_return_a_notification_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_delete_an_invalid_user : specification_for_users_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Delete(0, 5));

    It should_not_delete_the_user = () =>
      userRepository.AssertWasNotCalled(r => r.Delete(null), x => x.IgnoreArguments());

    It should_return_an_http_not_found_result = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_edit_a_user : specification_for_users_controller
  {
    static ViewResultHelper<EditViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(userId));

    It should_initialize_the_Username_field = () =>
      resultHelper.Model.Username.ShouldEqual(user.Username);

    It should_initialize_the_FirstName_field = () =>
      resultHelper.Model.FirstName.ShouldEqual(user.FirstName);

    It should_initialize_the_LastName_field = () =>
      resultHelper.Model.LastName.ShouldEqual(user.LastName);

    It should_initialize_the_Approved_field = () =>
      resultHelper.Model.IsApproved.ShouldEqual(user.IsApproved);

    It should_initialize_the_Locked_field = () =>
      resultHelper.Model.IsLocked.ShouldEqual(user.IsLocked);

    It should_initialize_the_Email_field = () =>
      resultHelper.Model.Email.ShouldEqual(user.Email);

    It should_initiaize_the_users_roles = () => 
      user.Roles.Each(r => resultHelper.Model.Roles.ShouldContain(r.Id));

    It should_initialize_the_available_roles = () => 
      roles.Each(r => resultHelper.Model.AvailableRoles.ShouldContain(new RoleViewModel(r)));
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_valid_data : specification_for_users_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static EditViewModel viewModel;
    static User user;
    static int userId;
    static List<Role> roles;

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

      user = new User("temp", "pass", "temp", "temp", "temp@temp.com") {
        IsLocked = !viewModel.IsLocked,
        IsApproved = !viewModel.IsApproved
      };
      user.SetIdTo(userId);

      roleRepository.Stub(r => r.GetAll()).Return(roles.AsQueryable());
      userRepository.Stub(r => r.Get(userId)).Return(user);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_redirect_to_the_default_users_view = () => {
      resultHelper.ShouldRedirectTo("users");
    };

    It should_return_a_notification_message = () => 
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_update_the_username = () =>
      user.Username.ShouldEqual(viewModel.Username);

    It should_update_the_first_name = () =>
      user.FirstName.ShouldEqual(viewModel.FirstName);

    It should_update_the_last_name = () =>
      user.LastName.ShouldEqual(viewModel.LastName);

    It should_update_the_email = () =>
      user.Email.ShouldEqual(viewModel.Email);

    It should_update_the_users_approved_status = () =>
      user.IsApproved.ShouldEqual(viewModel.IsApproved);

    It should_update_the_users_locked_status = () =>
      user.IsLocked.ShouldEqual(viewModel.IsLocked);

    It should_update_the_roles = () =>
      user.Roles.Each(r => viewModel.Roles.ShouldContain(r.Id));

    It should_not_update_the_id = () =>
      user.Id.ShouldEqual(viewModel.Id);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_a_model_state_error : specification_for_users_controller
  {
    static ViewResultHelper<EditViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(viewModel));

    It should_retain_the_data_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_username_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Username").ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_a_duplicate_username : specification_for_users_controller
  {
    static ViewResultHelper<EditViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(viewModel));

    It should_retain_the_data_already_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_username_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Username").ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_a_duplicate_email : specification_for_users_controller
  {
    static ViewResultHelper<EditViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(viewModel));

    It should_retain_the_data_already_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_email_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Email").ShouldBeTrue();
  }

  // we don't have to test much with Create because it's the same code as SignUp.
  // technically our specs shouldn't know this and exercise them both, but...
  [Subject(typeof(UsersController))]
  public class when_asked_to_create_a_user : specification_for_users_controller
  {
    static RedirectToRouteResultHelper resultHelper;
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

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));

    It should_create_an_approved_user = () =>
      user.IsApproved.ShouldBeTrue();

    It should_create_an_unlocked_user = () =>
      user.IsLocked.ShouldBeFalse();

    It should_redirect_to_the_default_users_view = () =>
      resultHelper.ShouldRedirectTo("users");
  }
}
