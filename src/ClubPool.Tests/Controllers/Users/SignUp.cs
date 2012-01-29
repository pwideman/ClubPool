using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Testing;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Users.when_asked_to_sign_up_a_new_user
{
  [TestFixture]
  public class with_an_invalid_view_model : UsersControllerTest
  {
    private ViewResultHelper<CreateViewModel> resultHelper;
    private CreateViewModel viewModel;

    public override void Given() {
      viewModel = new CreateViewModel();
      controller.ModelState.AddModelError("Username", new Exception("test"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateViewModel>(controller.SignUp(viewModel, true));
    }

    [Test]
    public void it_should_pass_the_data_provided_by_the_user_back_to_the_view() {
      resultHelper.Model.Should().Be(viewModel);
    }

    [Test]
    public void it_should_indicate_that_there_was_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }
  }

  [TestFixture]
  public class with_duplicate_username : UsersControllerTest
  {
    private ViewResultHelper<CreateViewModel> resultHelper;
    private CreateViewModel viewModel;
    private string username = "TestUser";

    public override void Given() {
      viewModel = new CreateViewModel();
      viewModel.Username = username;
      viewModel.Password = "test";
      viewModel.ConfirmPassword = "test";
      viewModel.Email = "test@test.com";
      viewModel.FirstName = "test";
      viewModel.LastName = "test";

      membershipService.Setup(s => s.UsernameIsInUse(username)).Returns(true);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateViewModel>(controller.SignUp(viewModel, true));
    }

    [Test]
    public void it_should_indicate_that_there_was_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_that_the_error_was_related_to_the_Username_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Username").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_data_that_the_user_has_already_entered() {
      resultHelper.Model.Should().Be(viewModel);
    }
  }

  [TestFixture]
  public class with_duplicate_email : UsersControllerTest
  {
    private ViewResultHelper<CreateViewModel> resultHelper;
    private CreateViewModel viewModel;
    private string email = "TestEmail@email.com";

    public override void Given() {
      viewModel = new CreateViewModel();
      viewModel.Username = "test";
      viewModel.Password = "test";
      viewModel.ConfirmPassword = "test";
      viewModel.Email = email;
      viewModel.FirstName = "test";
      viewModel.LastName = "test";

      membershipService.Setup(s => s.EmailIsInUse(email)).Returns(true);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateViewModel>(controller.SignUp(viewModel, true));
    }

    [Test]
    public void it_should_indicate_that_there_was_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_that_the_error_was_related_to_the_Email_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Email").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_data_that_the_user_has_already_entered() {
      resultHelper.Model.Should().Be(viewModel);
    }
  }

  [TestFixture]
  public class with_valid_info : UsersControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private CreateViewModel viewModel;
    private User user;
    private string username = "TestUser";
    private string userEmail = "test@test.com";

    public override void Given() {
      viewModel = new CreateViewModel();
      viewModel.Username = username;
      viewModel.Password = "test";
      viewModel.ConfirmPassword = "test";
      viewModel.Email = userEmail;
      viewModel.FirstName = "test";
      viewModel.LastName = "test";

      user = new User(viewModel.Username, viewModel.Password, viewModel.FirstName, viewModel.LastName, viewModel.Email);

      membershipService.Setup(s => s.CreateUser(username, viewModel.Password, viewModel.FirstName,
        viewModel.LastName, viewModel.Email, false, false)).Returns(user);

      var role = new Role(Roles.Administrators);
      user = new User("officer", "officer", "officer", "user", "officer@email.com");
      role.Users.Add(user);
      repository.Init<Role>(new List<Role> { role }.AsQueryable());
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.SignUp(viewModel, true));
    }

    [Test]
    public void it_should_return_the_SignUpComplete_view() {
      resultHelper.ShouldRedirectTo("SignUpComplete");
    }

    [Test]
    public void it_should_send_new_user_awaiting_approval_email_to_all_admins() {
      emailService.Verify(s => s.SendSystemEmail(It.IsAny<IList<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
    }
  }

  [TestFixture]
  public class and_the_captcha_is_invalid : UsersControllerTest
  {
    private ViewResultHelper<CreateViewModel> resultHelper;
    private CreateViewModel viewModel;
    private string username = "TestUser";

    public override void Given() {
      viewModel = new CreateViewModel();
      viewModel.Username = username;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateViewModel>(controller.SignUp(viewModel, false));
    }

    [Test]
    public void it_should_indicate_that_there_was_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_that_the_error_was_related_to_the_captcha_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("captcha").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_data_that_the_user_has_already_entered() {
      resultHelper.Model.Should().Be(viewModel);
    }
  }

}
