using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Models;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Users;
using ClubPool.Testing;

namespace ClubPool.Tests.Controllers.Users
{
  public abstract class EditPostTest : EditTest
  {
    protected EditViewModel viewModel;
    protected bool isApproved = true;
    protected bool isLocked = false;

    public override void EstablishContext() {
      base.EstablishContext();
      viewModel = new EditViewModel() {
        FirstName = "first",
        LastName = "last",
        Username = "newusername",
        Email = "newemail@email.com",
        IsLocked = !isLocked,
        IsApproved = !isApproved,
        Id = user.Id,
        Roles = new int[] { adminRole.Id, officerRole.Id },
        Version = DomainModelHelper.ConvertIntVersionToString(1),
        Password = "newpass",
        ConfirmPassword = "newpass"
      };

      user.IsLocked = isLocked;
      user.IsApproved = isApproved;

      membershipService.Setup(s => s.EncodePassword(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((password, salt) => password);
    }
  }
}

namespace ClubPool.Tests.Controllers.Users.when_asked_to_edit_a_user
{
  [TestFixture]
  public class by_an_admin_user : EditPostTest
  {
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_users_edit_view() {
      resultHelper.ShouldRedirectTo("edit");
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_update_the_username() {
      user.Username.Should().Be(viewModel.Username);
    }

    [Test]
    public void it_should_update_the_first_name() {
      user.FirstName.Should().Be(viewModel.FirstName);
    }

    [Test]
    public void it_should_update_the_last_name() {
      user.LastName.Should().Be(viewModel.LastName);
    }

    [Test]
    public void it_should_update_the_email() {
      user.Email.Should().Be(viewModel.Email);
    }

    [Test]
    public void it_should_update_the_users_approved_status() {
      user.IsApproved.Should().Be(viewModel.IsApproved);
    }

    [Test]
    public void it_should_update_the_users_locked_status() {
      user.IsLocked.Should().Be(viewModel.IsLocked);
    }

    [Test]
    public void it_should_update_the_roles() {
      user.Roles.Each(r => viewModel.Roles.Should().Contain(r.Id));
    }

    [Test]
    public void it_should_not_update_the_id() {
      user.Id.Should().Be(viewModel.Id);
    }

    [Test]
    public void it_should_update_the_password() {
      user.Password.Should().Be(viewModel.Password);
    }
  }

  [TestFixture]
  public class for_a_normal_user_by_an_officer_user : EditPostTest
  {
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_users_edit_view() {
      resultHelper.ShouldRedirectTo("edit");
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_update_the_username() {
      user.Username.Should().Be(viewModel.Username);
    }

    [Test]
    public void it_should_update_the_first_name() {
      user.FirstName.Should().Be(viewModel.FirstName);
    }

    [Test]
    public void it_should_update_the_last_name() {
      user.LastName.Should().Be(viewModel.LastName);
    }

    [Test]
    public void it_should_update_the_email() {
      user.Email.Should().Be(viewModel.Email);
    }

    [Test]
    public void it_should_update_the_approved_status() {
      user.IsApproved.Should().Be(viewModel.IsApproved);
    }

    [Test]
    public void it_should_update_the_locked_status() {
      user.IsLocked.Should().Be(viewModel.IsLocked);
    }

    [Test]
    public void it_should_not_update_the_roles() {
      user.Roles.Should().BeEmpty();
    }

    [Test]
    public void it_should_not_update_the_id() {
      user.Id.Should().Be(viewModel.Id);
    }

    [Test]
    public void it_should_not_update_the_password() {
      user.Password.Should().NotBe(viewModel.Password);
    }
  }

  [TestFixture]
  public class for_an_officer_user_by_a_different_officer_user : EditPostTest
  {
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      var officer = new User("officer", "test", "test", "test", "test");
      officer.SetIdTo(22);
      officer.AddRole(officerRole);
      authenticationService.MockPrincipal.User = officer;
      user.AddRole(officerRole);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_users_edit_view() {
      resultHelper.ShouldRedirectTo("edit");
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_update_the_username() {
      user.Username.Should().Be(viewModel.Username);
    }

    [Test]
    public void it_should_update_the_first_name() {
      user.FirstName.Should().Be(viewModel.FirstName);
    }

    [Test]
    public void it_should_update_the_last_name() {
      user.LastName.Should().Be(viewModel.LastName);
    }

    [Test]
    public void it_should_update_the_email() {
      user.Email.Should().Be(viewModel.Email);
    }

    [Test]
    public void it_should_update_the_approved_status() {
      user.IsApproved.Should().Be(viewModel.IsApproved);
    }

    [Test]
    public void it_should_update_the_locked_status() {
      user.IsLocked.Should().Be(viewModel.IsLocked);
    }

    [Test]
    public void it_should_not_update_the_roles() {
      user.Roles.Count().Should().Be(1);
    }

    [Test]
    public void it_should_not_update_the_id() {
      user.Id.Should().Be(viewModel.Id);
    }

    [Test]
    public void it_should_not_update_the_password() {
      user.Password.Should().NotBe(viewModel.Password);
    }
  }

  [TestFixture]
  public class for_an_officer_user_by_himself : EditPostTest
  {
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      authenticationService.MockPrincipal.User = user;
      user.AddRole(officerRole);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_users_edit_view() {
      resultHelper.ShouldRedirectTo("edit");
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_update_the_username() {
      user.Username.Should().Be(viewModel.Username);
    }

    [Test]
    public void it_should_update_the_first_name() {
      user.FirstName.Should().Be(viewModel.FirstName);
    }

    [Test]
    public void it_should_update_the_last_name() {
      user.LastName.Should().Be(viewModel.LastName);
    }

    [Test]
    public void it_should_update_the_email() {
      user.Email.Should().Be(viewModel.Email);
    }

    [Test]
    public void it_should_not_update_the_approved_status() {
      user.IsApproved.Should().Be(isApproved);
    }

    [Test]
    public void it_should_not_update_the_locked_status() {
      user.IsLocked.Should().Be(isLocked);
    }

    [Test]
    public void it_should_not_update_the_roles() {
      user.Roles.Count().Should().Be(1);
    }

    [Test]
    public void it_should_not_update_the_id() {
      user.Id.Should().Be(viewModel.Id);
    }

    [Test]
    public void it_should_update_the_password() {
      user.Password.Should().Be(viewModel.Password);
    }
  }

  [TestFixture]
  public class for_a_normal_user_by_the_user : EditPostTest
  {
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[0];
      authenticationService.MockPrincipal.User = user;
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_users_edit_view() {
      resultHelper.ShouldRedirectTo("edit");
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_update_the_username() {
      user.Username.Should().Be(viewModel.Username);
    }

    [Test]
    public void it_should_update_the_first_name() {
      user.FirstName.Should().Be(viewModel.FirstName);
    }

    [Test]
    public void it_should_update_the_last_name() {
      user.LastName.Should().Be(viewModel.LastName);
    }

    [Test]
    public void it_should_update_the_email() {
      user.Email.Should().Be(viewModel.Email);
    }

    [Test]
    public void it_should_not_update_the_users_approved_status() {
      user.IsApproved.Should().Be(isApproved);
    }

    [Test]
    public void it_should_not_update_the_users_locked_status() {
      user.IsLocked.Should().Be(isLocked);
    }

    [Test]
    public void it_should_not_update_the_roles() {
      user.Roles.Each(r => viewModel.Roles.Should().Contain(r.Id));
    }

    [Test]
    public void it_should_not_update_the_id() {
      user.Id.Should().Be(viewModel.Id);
    }

    [Test]
    public void it_should_update_the_password() {
      user.Password.Should().Be(viewModel.Password);
    }
  }

  [TestFixture]
  public class by_normal_user_trying_to_edit_another_user : EditPostTest
  {
    private ViewResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[0];
      var tempUser = new User("test", "test", "test", "test", "test");
      tempUser.SetIdTo(22);
      authenticationService.MockPrincipal.User = tempUser;
    }

    public override void When() {
      resultHelper = new ViewResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_return_the_error_view() {
      resultHelper.Result.ViewName.Should().Be("Error");
    }

    [Test]
    public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class with_a_stale_version : EditPostTest
  {
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      user.SetVersionTo(2);
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_edit_view() {
      resultHelper.ShouldRedirectTo("edit");
    }

    [Test]
    public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class with_an_invalid_id : EditPostTest
  {
    private ViewResultHelper resultHelper;

    public override void Given() {
      viewModel.Id = 2;
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    }

    public override void When() {
      resultHelper = new ViewResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_return_error_view() {
      resultHelper.Result.ViewName.Should().Be("Error");
    }

    [Test]
    public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class with_a_model_state_error : EditPostTest
  {
    private ViewResultHelper<EditViewModel> resultHelper;

    public override void Given() {
      viewModel.Username = null;
      controller.ModelState.AddModelError("Username", new Exception("test"));
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_retain_the_data_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_username_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Username").Should().BeTrue();
    }
  }

  [TestFixture]
  public class with_a_duplicate_username : EditPostTest
  {
    private ViewResultHelper<EditViewModel> resultHelper;

    public override void Given() {
      membershipService.Setup(s => s.UsernameIsInUse(It.IsAny<string>())).Returns(true);
      repository.Setup(r => r.All<Role>()).Returns(new List<Role>().AsQueryable());
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_retain_the_data_already_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_username_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Username").Should().BeTrue();
    }
  }

  [TestFixture]
  public class with_a_duplicate_email : EditPostTest
  {
    private ViewResultHelper<EditViewModel> resultHelper;

    public override void Given() {
      membershipService.Setup(s => s.EmailIsInUse(It.IsAny<string>())).Returns(true);
      repository.Setup(r => r.All<Role>()).Returns(new List<Role>().AsQueryable());
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_retain_the_data_already_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_email_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Email").Should().BeTrue();
    }
  }

}
