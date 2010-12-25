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

using ClubPool.ApplicationServices.Configuration.Contracts;
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
using ClubPool.Testing;
using ClubPool.Framework.Configuration;

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
    protected static IConfigurationService configService;

    Establish context = () => {
      roleRepository = MockRepository.GenerateStub<IRoleRepository>();
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      membershipService = MockRepository.GenerateStub<IMembershipService>();
      userRepository = MockRepository.GenerateStub<IUserRepository>();
      emailService = MockRepository.GenerateStub<IEmailService>();
      configService = MockRepository.GenerateStub<IConfigurationService>();
      var config = new ClubPoolConfiguration("test", "test", "test@test.com", false);
      configService.Stub(c => c.GetConfig()).Return(config);
      controller = new UsersController(authenticationService, membershipService, emailService, userRepository, roleRepository, configService);
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
      resultHelper.Result.ViewData.ModelState.Keys.ShouldContain("ConfirmPassword");
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
      users.Where(u => !u.IsApproved).Each(u => resultHelper.Model.UnapprovedUsers.Where(uu => uu.Id == u.Id).Any().ShouldBeTrue());
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_approve_users : specification_for_users_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static IList<User> users;
    static List<string> emails;
    static int emailAttempts;

    Establish context = () => {
      users = new List<User>() {
        new User("user1", "user1", "user", "one", "user@user.com").SetIdTo(1) as User,
        new User("user2", "user2", "user", "two", "user2@user.com").SetIdTo(2) as User,
        new User("user3", "user3", "user", "three", "user3@user.com").SetIdTo(3) as User
      };
      userRepository.Stub(r => r.GetAll()).Return(users.AsQueryable());
      emails = new List<string>();
      emailService.Stub(s => s.SendSystemEmail("", "", "")).IgnoreArguments().WhenCalled(m => {
        emailAttempts++;
        var email = (string)m.Arguments[0];
        emails.Add(email);
      });
      controller.ControllerContext.HttpContext.Request.Stub(r => r.Url).Return(new Uri("http://host/users/unapproved"));
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

    It should_send_an_email_to_each_user = () =>
      emailAttempts.ShouldEqual(2);
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

  [Subject(typeof(UsersController))]
  public class when_asked_for_the_user_view_for_an_invalid_user : specification_for_users_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.View(0));

    It should_return_http_not_found = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(UsersController))]
  public class when_a_nonadmin_user_asks_for_the_user_view : specification_for_users_controller
  {
    static ViewResultHelper<ViewViewModel> resultHelper;
    static int id = 1;
    static string username = "test";

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      userRepository.Stub(r => r.Get(id)).Return(new User(username, "test", "first", "last", "email"));
    };

    Because of = () => resultHelper = new ViewResultHelper<ViewViewModel>(controller.View(id));

    It should_return_the_correct_user = () =>
      resultHelper.Model.Username.ShouldEqual(username);

    It should_not_show_admin_properties = () =>
      resultHelper.Model.ShowAdminProperties.ShouldBeFalse();
  }

  [Subject(typeof(UsersController))]
  public class when_an_admin_user_asks_for_the_user_view : specification_for_users_controller
  {
    static ViewResultHelper<ViewViewModel> resultHelper;
    static int id = 1;
    static string username = "test";

    Establish context = () => {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      authenticationService.MockPrincipal.Roles = new string[] { Roles.Administrators };
      userRepository.Stub(r => r.Get(id)).Return(new User(username, "test", "first", "last", "email"));
    };

    Because of = () => resultHelper = new ViewResultHelper<ViewViewModel>(controller.View(id));

    It should_return_the_correct_user = () =>
      resultHelper.Model.Username.ShouldEqual(username);

    It should_show_admin_properties = () =>
      resultHelper.Model.ShowAdminProperties.ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_reset_a_password : specification_for_users_controller
  {
    static ViewResultHelper resultHelper;
    static ResetPasswordViewModel viewModel;
    static string username = "test";
    static string token = "testtoken";
    static User user;
    static string emailTo;
    static string emailSubject;
    static string emailBody;

    Establish context = () => {
      viewModel = new ResetPasswordViewModel() { Username = username };
      user = new User(username, "test", "test", "user", "test") { Password = "before", PasswordSalt = "salt" };
      userRepository.Stub(r => r.FindOne(null)).IgnoreArguments().Return(user);
      membershipService.Stub(s => s.GeneratePasswordResetToken(user)).IgnoreArguments().Return(token);
      emailService.Stub(s => s.SendSystemEmail("", null, null)).IgnoreArguments()
        .WhenCalled(m => { 
          emailTo = m.Arguments[0] as string;
          emailSubject = m.Arguments[1] as string;
          emailBody = m.Arguments[2] as string;
        });
      controller.ControllerContext.HttpContext.Request.Stub(r => r.Url).Return(new Uri("http://host/users/resetpassword"));
    };

    Because of = () => resultHelper = new ViewResultHelper(controller.ResetPassword(viewModel, true));

    It should_return_the_reset_password_complete_view = () =>
      resultHelper.Result.ViewName.ShouldEqual("ResetPasswordComplete");

    It should_send_the_user_an_email = () =>
      emailTo.ShouldEqual(user.Email);

    // cannot assert this until I figure out how to mock the controller context
    // enough to allow UrlHelper to work
    //It should_send_the_token_in_the_email = () =>
    //  emailBody.ShouldContain(token);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_reset_a_password_for_a_nonexistent_username : specification_for_users_controller
  {
    static ResetPasswordViewModel viewModel;
    static ViewResultHelper resultHelper;

    Establish context = () => {
      viewModel = new ResetPasswordViewModel() { Username = "bad" };
    };

    Because of = () => resultHelper = new ViewResultHelper(controller.ResetPassword(viewModel, true));

    It should_return_the_reset_password_complete_view = () =>
      resultHelper.Result.ViewName.ShouldEqual("ResetPasswordComplete");

    It should_not_send_an_email = () =>
      emailService.AssertWasNotCalled(s => s.SendSystemEmail("", null, null), c => c.IgnoreArguments());
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_reset_a_password_with_an_invalid_view_model : specification_for_users_controller
  {
    static ResetPasswordViewModel viewModel;
    static ViewResultHelper<ResetPasswordViewModel> resultHelper;

    Establish context = () => {
      viewModel = new ResetPasswordViewModel();
    };

    Because of = () => resultHelper = new ViewResultHelper<ResetPasswordViewModel>(controller.ResetPassword(viewModel, true));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_return_a_page_error_message = () =>
      resultHelper.Result.TempData.Keys.ShouldContain(GlobalViewDataProperty.PageErrorMessage);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_reset_a_password_with_an_invalid_captcha : specification_for_users_controller
  {
    static ResetPasswordViewModel viewModel;
    static ViewResultHelper<ResetPasswordViewModel> resultHelper;

    Establish context = () => {
      viewModel = new ResetPasswordViewModel();
      viewModel.Username = "test";
    };

    Because of = () => resultHelper = new ViewResultHelper<ResetPasswordViewModel>(controller.ResetPassword(viewModel, false));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_return_a_validation_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_return_a_validation_error_for_the_captcha_field = () =>
      resultHelper.Result.ViewData.ModelState.Keys.ShouldContain("captcha");
  }


  [Subject(typeof(UsersController))]
  public class when_asked_to_recover_username : specification_for_users_controller
  {
    static ViewResultHelper resultHelper;
    static RecoverUsernameViewModel viewModel;
    static string email = "test@email.com";
    static string username = "testusername";
    static User user;
    static string emailTo;
    static string emailSubject;
    static string emailBody;

    Establish context = () => {
      viewModel = new RecoverUsernameViewModel() { Email = email };
      user = new User(username, "test", "test", "user", email);
      userRepository.Stub(r => r.GetAll()).Return(new List<User>() { user }.AsQueryable());
      emailService.Stub(s => s.SendSystemEmail("", null, null)).IgnoreArguments()
        .WhenCalled(m => {
          emailTo = m.Arguments[0] as string;
          emailSubject = m.Arguments[1] as string;
          emailBody = m.Arguments[2] as string;
        });
    };

    Because of = () => resultHelper = new ViewResultHelper(controller.RecoverUsername(viewModel, true));

    It should_return_the_recover_username_complete_view = () =>
      resultHelper.Result.ViewName.ShouldEqual("RecoverUsernameComplete");

    It should_send_the_user_an_email = () =>
      emailTo.ShouldEqual(user.Email);

    It should_send_the_usernames_in_the_email = () =>
      emailBody.ShouldContain(username);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_recover_username_for_a_nonexistent_email : specification_for_users_controller
  {
    static RecoverUsernameViewModel viewModel;
    static ViewResultHelper resultHelper;
    static string emailTo;
    static string testEmail = "bad@email.com";

    Establish context = () => {
      viewModel = new RecoverUsernameViewModel() { Email = testEmail };
      var user = new User("test", "test", "test", "user", "test");
      userRepository.Stub(r => r.GetAll()).Return(new List<User>() { user }.AsQueryable());
      emailService.Stub(s => s.SendSystemEmail("", null, null)).IgnoreArguments()
        .WhenCalled(m => {
          emailTo = m.Arguments[0] as string;
        });
    };

    Because of = () => resultHelper = new ViewResultHelper(controller.RecoverUsername(viewModel, true));

    It should_return_the_recover_username_complete_view = () =>
      resultHelper.Result.ViewName.ShouldEqual("RecoverUsernameComplete");

    It should_send_the_user_an_email = () =>
      emailTo.ShouldEqual(testEmail);
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_recover_a_username_with_an_invalid_view_model : specification_for_users_controller
  {
    static RecoverUsernameViewModel viewModel;
    static ViewResultHelper<RecoverUsernameViewModel> resultHelper;

    Establish context = () => {
      viewModel = new RecoverUsernameViewModel();
    };

    Because of = () => resultHelper = new ViewResultHelper<RecoverUsernameViewModel>(controller.RecoverUsername(viewModel, true));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_add_a_model_state_error_for_email = () =>
      resultHelper.Result.ViewData.ModelState.Keys.ShouldContain("Email");
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_recover_a_username_with_an_invalid_captcha : specification_for_users_controller
  {
    static RecoverUsernameViewModel viewModel;
    static ViewResultHelper<RecoverUsernameViewModel> resultHelper;

    Establish context = () => {
      viewModel = new RecoverUsernameViewModel();
      viewModel.Email = "test@test.com";
    };

    Because of = () => resultHelper = new ViewResultHelper<RecoverUsernameViewModel>(controller.RecoverUsername(viewModel, false));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_return_a_validation_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_return_a_validation_error_for_the_captcha_field = () =>
      resultHelper.Result.ViewData.ModelState.Keys.ShouldContain("captcha");
  }

}
