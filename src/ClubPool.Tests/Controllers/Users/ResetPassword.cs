using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Users.when_asked_to_reset_a_password
{
  [TestFixture]
  public class with_valid_input : UsersControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private ResetPasswordViewModel viewModel;
    private string username = "test";
    private string token = "testtoken";
    private User user;
    private string emailTo;
    private string emailSubject;
    private string emailBody;

    public override void Given() {
      viewModel = new ResetPasswordViewModel() { Username = username };
      user = new User(username, "test", "test", "user", "test") { Password = "before", PasswordSalt = "salt" };
      repository.Init(new List<User> { user }.AsQueryable());
      membershipService.Setup(s => s.GeneratePasswordResetToken(user)).Returns(token);
      emailService.Setup(s => s.SendSystemEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Callback<string, string, string>((to, subject, body) => {
          emailTo = to;
          emailSubject = subject;
          emailBody = body;
        });
      ControllerHelper.CreateMockControllerContext(controller);
      Mock.Get(controller.ControllerContext.HttpContext.Request).Setup(r => r.Url).Returns(new Uri("http://host/users/resetpassword"));
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.ResetPassword(viewModel, true));
    }

    [Test]
    public void it_should_return_the_reset_password_complete_view() {
      resultHelper.ShouldRedirectTo("ResetPasswordComplete");
    }

    [Test]
    public void it_should_send_the_user_an_email() {
      emailTo.Should().Be(user.Email);
    }

    // cannot assert this until I figure out how to mock the controller context
    // enough to allow UrlHelper to work
    //It should_send_the_token_in_the_email = () =>
    //  emailBody.ShouldContain(token);
  }

  [TestFixture]
  public class for_a_nonexistent_username : UsersControllerTest
  {
    private ResetPasswordViewModel viewModel;
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      viewModel = new ResetPasswordViewModel() { Username = "bad" };
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.ResetPassword(viewModel, true));
    }

    [Test]
    public void it_should_return_the_reset_password_complete_view() {
      resultHelper.ShouldRedirectTo("ResetPasswordComplete");
    }

    [Test]
    public void it_should_not_send_an_email() {
      emailService.Verify(s => s.SendSystemEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }
  }

  [TestFixture]
  public class with_an_invalid_view_model : UsersControllerTest
  {
    private ResetPasswordViewModel viewModel;
    private ViewResultHelper<ResetPasswordViewModel> resultHelper;

    public override void Given() {
      viewModel = new ResetPasswordViewModel();
    }

    public override void When() {
      resultHelper = new ViewResultHelper<ResetPasswordViewModel>(controller.ResetPassword(viewModel, true));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeNullOrEmpty();
    }

    [Test]
    public void it_should_return_a_page_error_message() {
      resultHelper.Result.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class with_an_invalid_captcha : UsersControllerTest
  {
    private ResetPasswordViewModel viewModel;
    private ViewResultHelper<ResetPasswordViewModel> resultHelper;

    public override void Given() {
      viewModel = new ResetPasswordViewModel();
      viewModel.Username = "test";
    }

    public override void When() {
      resultHelper = new ViewResultHelper<ResetPasswordViewModel>(controller.ResetPassword(viewModel, false));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_return_a_validation_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_return_a_validation_error_for_the_captcha_field() {
      resultHelper.Result.ViewData.ModelState.Keys.Should().Contain("captcha");
    }
  }


}
