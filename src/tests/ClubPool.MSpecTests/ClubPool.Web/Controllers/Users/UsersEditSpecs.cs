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
using ClubPool.Testing;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Users
{
  public abstract class specification_for_users_edit : specification_for_users_controller
  {
    protected static int userId = 1;
    protected static User user;
    protected static List<Role> roles;
    protected static Role adminRole;
    protected static Role officerRole;
    protected static string password = "pass";

    Establish context = () => {
      roles = new List<Role>();
      adminRole = new Role(Roles.Administrators);
      adminRole.SetIdTo(1);
      roles.Add(adminRole);
      officerRole = new Role(Roles.Officers);
      officerRole.SetIdTo(2);
      roles.Add(officerRole);
      user = new User("user", password, "user", "one", "user@user.com");
      user.SetIdTo(userId);
      user.SetVersionTo(1);
      userRepository.Stub(r => r.Get(userId)).Return(user);
      roleRepository.Stub(r => r.GetAll()).Return(roles.AsQueryable());
      foreach (var role in roles) {
        roleRepository.Stub(r => r.Get(role.Id)).Return(role);
      }
    };
  }

  public class specification_for_users_edit_post : specification_for_users_edit
  {
    protected static EditViewModel viewModel;
    protected static bool isApproved = true;
    protected static bool isLocked = false;

    Establish context = () => {
      viewModel = new EditViewModel() {
        FirstName = "first",
        LastName = "last",
        Username = "newusername",
        Email = "newemail@email.com",
        IsLocked = !isLocked,
        IsApproved = !isApproved,
        Id = userId,
        Roles = new int[] { adminRole.Id, officerRole.Id },
        Version = 1,
        Password = "newpass",
        ConfirmPassword = "newpass"
      };

      user.IsLocked = isLocked;
      user.IsApproved = isApproved;

      membershipService.Stub(s => s.EncodePassword(null, null)).IgnoreArguments().Return(null).WhenCalled(
        m => m.ReturnValue = m.Arguments[0]);
    };
  }


  [Subject(typeof(UsersController))]
  public class when_asked_to_edit_a_user_by_an_admin_user : specification_for_users_edit
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    };

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(userId));
    
    It should_initialize_the_Username_field = () =>
      resultHelper.Model.Username.ShouldEqual(user.Username);

    It should_initialize_the_FirstName_field = () =>
      resultHelper.Model.FirstName.ShouldEqual(user.FirstName);

    It should_initialize_the_LastName_field = () =>
      resultHelper.Model.LastName.ShouldEqual(user.LastName);

    It should_show_the_status_fields = () =>
      resultHelper.Model.ShowStatus.ShouldBeTrue();

    It should_initialize_the_Approved_field = () =>
      resultHelper.Model.IsApproved.ShouldEqual(user.IsApproved);

    It should_initialize_the_Locked_field = () =>
      resultHelper.Model.IsLocked.ShouldEqual(user.IsLocked);

    It should_initialize_the_Email_field = () =>
      resultHelper.Model.Email.ShouldEqual(user.Email);

    It should_show_the_roles_fields = () =>
      resultHelper.Model.ShowRoles.ShouldBeTrue();

    It should_initialize_the_users_roles = () =>
      user.Roles.Each(r => resultHelper.Model.Roles.ShouldContain(r.Id));

    It should_initialize_the_available_roles = () =>
      roles.Each(r => resultHelper.Model.AvailableRoles.Where(ar => ar.Id == r.Id).Any().ShouldBeTrue());

    It should_not_show_the_password = () =>
      resultHelper.Model.ShowPassword.ShouldBeFalse();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_edit_a_normal_user_by_an_officer : specification_for_users_edit
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
    };

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(userId));

    It should_initialize_the_Username_field = () =>
      resultHelper.Model.Username.ShouldEqual(user.Username);

    It should_initialize_the_FirstName_field = () =>
      resultHelper.Model.FirstName.ShouldEqual(user.FirstName);

    It should_initialize_the_LastName_field = () =>
      resultHelper.Model.LastName.ShouldEqual(user.LastName);

    It should_show_the_status_fields = () =>
      resultHelper.Model.ShowStatus.ShouldBeTrue();

    It should_initialize_the_Approved_field = () =>
      resultHelper.Model.IsApproved.ShouldEqual(user.IsApproved);

    It should_initialize_the_Locked_field = () =>
      resultHelper.Model.IsLocked.ShouldEqual(user.IsLocked);

    It should_initialize_the_Email_field = () =>
      resultHelper.Model.Email.ShouldEqual(user.Email);

    It should_not_show_the_roles_fields = () =>
      resultHelper.Model.ShowRoles.ShouldBeFalse();

    It should_not_show_the_password = () =>
      resultHelper.Model.ShowPassword.ShouldBeFalse();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_edit_an_officer_by_a_different_officer : specification_for_users_edit
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      user.AddRole(officerRole);
    };

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(userId));

    It should_initialize_the_Username_field = () =>
      resultHelper.Model.Username.ShouldEqual(user.Username);

    It should_initialize_the_FirstName_field = () =>
      resultHelper.Model.FirstName.ShouldEqual(user.FirstName);

    It should_initialize_the_LastName_field = () =>
      resultHelper.Model.LastName.ShouldEqual(user.LastName);

    It should_show_the_status_fields = () =>
      resultHelper.Model.ShowStatus.ShouldBeTrue();

    It should_initialize_the_Approved_field = () =>
      resultHelper.Model.IsApproved.ShouldEqual(user.IsApproved);

    It should_initialize_the_Locked_field = () =>
      resultHelper.Model.IsLocked.ShouldEqual(user.IsLocked);

    It should_initialize_the_Email_field = () =>
      resultHelper.Model.Email.ShouldEqual(user.Email);

    It should_not_show_the_roles_fields = () =>
      resultHelper.Model.ShowRoles.ShouldBeFalse();

    It should_not_show_the_password = () =>
      resultHelper.Model.ShowPassword.ShouldBeFalse();
  }

  [Subject(typeof(UsersController))]
  public class when_an_officer_asks_to_edit_himself : specification_for_users_edit
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      authenticationService.MockPrincipal.User = user;
      user.AddRole(officerRole);
    };

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(userId));

    It should_initialize_the_Username_field = () =>
      resultHelper.Model.Username.ShouldEqual(user.Username);

    It should_initialize_the_FirstName_field = () =>
      resultHelper.Model.FirstName.ShouldEqual(user.FirstName);

    It should_initialize_the_LastName_field = () =>
      resultHelper.Model.LastName.ShouldEqual(user.LastName);

    It should_initialize_the_Email_field = () =>
      resultHelper.Model.Email.ShouldEqual(user.Email);

    It should_not_show_the_status_fields = () =>
      resultHelper.Model.ShowStatus.ShouldBeFalse();

    It should_not_show_the_roles_fields = () =>
      resultHelper.Model.ShowRoles.ShouldBeFalse();

    It should_show_the_password = () =>
      resultHelper.Model.ShowPassword.ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_a_normal_user_asks_to_edit_himself : specification_for_users_edit
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[0];
      authenticationService.MockPrincipal.User = user;
    };

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(userId));

    It should_initialize_the_Username_field = () =>
      resultHelper.Model.Username.ShouldEqual(user.Username);

    It should_initialize_the_FirstName_field = () =>
      resultHelper.Model.FirstName.ShouldEqual(user.FirstName);

    It should_initialize_the_LastName_field = () =>
      resultHelper.Model.LastName.ShouldEqual(user.LastName);

    It should_initialize_the_Email_field = () =>
      resultHelper.Model.Email.ShouldEqual(user.Email);

    It should_not_show_the_status_fields = () =>
      resultHelper.Model.ShowStatus.ShouldBeFalse();

    It should_not_show_the_roles_fields = () =>
      resultHelper.Model.ShowRoles.ShouldBeFalse();

    It should_show_the_password = () =>
      resultHelper.Model.ShowPassword.ShouldBeTrue();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_edit_an_admin_user_by_an_officer : specification_for_users_edit
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      user.AddRole(adminRole);
    };

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(userId));

    It should_initialize_the_Username_field = () =>
      resultHelper.Model.Username.ShouldEqual(user.Username);

    It should_initialize_the_FirstName_field = () =>
      resultHelper.Model.FirstName.ShouldEqual(user.FirstName);

    It should_initialize_the_LastName_field = () =>
      resultHelper.Model.LastName.ShouldEqual(user.LastName);

    It should_initialize_the_Email_field = () =>
      resultHelper.Model.Email.ShouldEqual(user.Email);

    It should_not_show_the_status_fields = () =>
      resultHelper.Model.ShowStatus.ShouldBeFalse();

    It should_not_show_the_roles_fields = () =>
      resultHelper.Model.ShowRoles.ShouldBeFalse();

    It should_not_show_the_password = () =>
      resultHelper.Model.ShowPassword.ShouldBeFalse();
  }

  [Subject(typeof(UsersController))]
  public class when_asked_to_edit_a_user_by_a_different_normal_user : specification_for_users_edit
  {
    static ViewResultHelper resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[0];
      authenticationService.MockPrincipal.MockIdentity.Name = "test";
    };

    Because of = () => resultHelper = new ViewResultHelper(controller.Edit(userId));

    It should_return_the_error_view = () =>
      resultHelper.Result.ViewName.ShouldEqual("Error");

    It should_indicate_an_error = () =>
    controller.TempData.Keys.ShouldContain(GlobalViewDataProperty.PageErrorMessage);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_by_admin_user : specification_for_users_edit_post
  {
    static RedirectToRouteResultHelper resultHelper;

    Establish context = () =>
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_redirect_to_the_users_edit_view = () =>
      resultHelper.ShouldRedirectTo("users", "edit");

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

    It should_not_update_the_password = () =>
      user.Password.ShouldNotEqual(viewModel.Password);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_for_a_normal_user_by_an_officer_user : specification_for_users_edit_post
  {
    static RedirectToRouteResultHelper resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_redirect_to_the_users_edit_view = () =>
      resultHelper.ShouldRedirectTo("users", "edit");

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

    It should_update_the_approved_status = () =>
      user.IsApproved.ShouldEqual(viewModel.IsApproved);

    It should_update_the_locked_status = () =>
      user.IsLocked.ShouldEqual(viewModel.IsLocked);

    It should_not_update_the_roles = () =>
      user.Roles.ShouldBeEmpty();

    It should_not_update_the_id = () =>
      user.Id.ShouldEqual(viewModel.Id);

    It should_not_update_the_password = () =>
      user.Password.ShouldNotEqual(viewModel.Password);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_for_an_officer_user_by_a_different_officer_user : specification_for_users_edit_post
  {
    static RedirectToRouteResultHelper resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      var officer = new User("officer", "test", "test", "test", "test");
      officer.SetIdTo(22);
      officer.AddRole(officerRole);
      authenticationService.MockPrincipal.User = officer;
      user.AddRole(officerRole);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_redirect_to_the_users_edit_view = () =>
      resultHelper.ShouldRedirectTo("users", "edit");

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

    It should_update_the_approved_status = () =>
      user.IsApproved.ShouldEqual(viewModel.IsApproved);

    It should_update_the_locked_status = () =>
      user.IsLocked.ShouldEqual(viewModel.IsLocked);

    It should_not_update_the_roles = () =>
      user.Roles.Count().ShouldEqual(1);

    It should_not_update_the_id = () =>
      user.Id.ShouldEqual(viewModel.Id);

    It should_not_update_the_password = () =>
      user.Password.ShouldNotEqual(viewModel.Password);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_for_an_officer_user_by_himself : specification_for_users_edit_post
  {
    static RedirectToRouteResultHelper resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      authenticationService.MockPrincipal.User = user;
      user.AddRole(officerRole);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_redirect_to_the_users_edit_view = () =>
      resultHelper.ShouldRedirectTo("users", "edit");

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

    It should_not_update_the_approved_status = () =>
      user.IsApproved.ShouldEqual(isApproved);

    It should_not_update_the_locked_status = () =>
      user.IsLocked.ShouldEqual(isLocked);

    It should_not_update_the_roles = () =>
      user.Roles.Count().ShouldEqual(1);

    It should_not_update_the_id = () =>
      user.Id.ShouldEqual(viewModel.Id);

    It should_update_the_password = () =>
      user.Password.ShouldEqual(viewModel.Password);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_for_a_normal_user_by_the_user : specification_for_users_edit_post
  {
    static RedirectToRouteResultHelper resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[0];
      authenticationService.MockPrincipal.User = user;
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_redirect_to_the_users_edit_view = () =>
      resultHelper.ShouldRedirectTo("users", "edit");

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

    It should_not_update_the_users_approved_status = () =>
      user.IsApproved.ShouldEqual(isApproved);

    It should_not_update_the_users_locked_status = () =>
      user.IsLocked.ShouldEqual(isLocked);

    It should_not_update_the_roles = () =>
      user.Roles.Each(r => viewModel.Roles.ShouldContain(r.Id));

    It should_not_update_the_id = () =>
      user.Id.ShouldEqual(viewModel.Id);

    It should_update_the_password = () =>
      user.Password.ShouldEqual(viewModel.Password);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_by_normal_user_trying_to_edit_another_user : specification_for_users_edit_post
  {
    static ViewResultHelper resultHelper;

    Establish context = () => {
      authenticationService.MockPrincipal.Roles = new string[0];
      var tempUser = new User("test", "test", "test", "test", "test");
      tempUser.SetIdTo(22);
      authenticationService.MockPrincipal.User = tempUser;
    };

    Because of = () => resultHelper = new ViewResultHelper(controller.Edit(viewModel));

    It should_return_the_error_view = () =>
      resultHelper.Result.ViewName.ShouldEqual("Error");

    It should_indicate_an_error = () =>
    controller.TempData.Keys.ShouldContain(GlobalViewDataProperty.PageErrorMessage);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_a_stale_version : specification_for_users_edit_post
  {
    static RedirectToRouteResultHelper resultHelper;
    static int version = 2;

    Establish context = () => {
      user.SetVersionTo(version);
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_redirect_to_the_edit_view = () =>
      resultHelper.ShouldRedirectTo("users", "edit");

    It should_indicate_an_error = () =>
      controller.TempData.Keys.ShouldContain(GlobalViewDataProperty.PageErrorMessage);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_an_invalid_id : specification_for_users_edit_post
  {
    static ViewResultHelper resultHelper;

    Establish context = () => {
      viewModel.Id = 2;
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    };

    Because of = () => resultHelper = new ViewResultHelper(controller.Edit(viewModel));

    It should_return_error_view = () =>
      resultHelper.Result.ViewName.ShouldEqual("Error");

    It should_indicate_an_error = () =>
      controller.TempData.Keys.ShouldContain(GlobalViewDataProperty.PageErrorMessage);
  }

  [Subject(typeof(UsersController))]
  public class when_the_edit_form_is_posted_with_a_model_state_error : specification_for_users_edit_post
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      viewModel.Username = null;
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
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
  public class when_the_edit_form_is_posted_with_a_duplicate_username : specification_for_users_edit_post
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      userRepository.Stub(r => r.DbContext).Return(MockRepository.GenerateStub<SharpArch.Core.PersistenceSupport.IDbContext>());
      membershipService.Stub(s => s.UsernameIsInUse(null)).IgnoreArguments().Return(true);
      roleRepository.Stub(s => s.GetAll()).Return(new List<Role>().AsQueryable());
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
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
  public class when_the_edit_form_is_posted_with_a_duplicate_email : specification_for_users_edit_post
  {
    static ViewResultHelper<EditViewModel> resultHelper;

    Establish context = () => {
      userRepository.Stub(r => r.DbContext).Return(MockRepository.GenerateStub<SharpArch.Core.PersistenceSupport.IDbContext>());
      membershipService.Stub(s => s.EmailIsInUse(null)).IgnoreArguments().Return(true);
      roleRepository.Stub(s => s.GetAll()).Return(new List<Role>().AsQueryable());
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    };

    Because of = () => resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(viewModel));

    It should_retain_the_data_already_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_email_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Email").ShouldBeTrue();
  }
}
