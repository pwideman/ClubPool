using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Users.when_asked_to_recover_username
{
  [TestFixture]
  public class with_valid_inputs : UsersControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private RecoverUsernameViewModel viewModel;
    private string email = "test@email.com";
    private string username = "testusername";
    private User user;
    private string emailTo;
    private string emailSubject;
    private string emailBody;

    public override void Given() {
      viewModel = new RecoverUsernameViewModel() { Email = email };
      user = new User(username, "test", "test", "user", email);
      repository.Init(new List<User> { user }.AsQueryable());
      emailService.Setup(s => s.SendSystemEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Callback<string, string, string>((to, subject, body) => {
          emailTo = to;
          emailSubject = subject;
          emailBody = body;
        });
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.RecoverUsername(viewModel, true));
    }

    [Test]
    public void it_should_return_the_recover_username_complete_view() {
      resultHelper.ShouldRedirectTo("RecoverUsernameComplete");
    }

    [Test]
    public void it_should_send_the_user_an_email() {
      emailTo.Should().Be(user.Email);
    }

    [Test]
    public void it_should_send_the_usernames_in_the_email() {
      emailBody.Should().Contain(username);
    }
  }

  [TestFixture]
  public class for_a_nonexistent_email : UsersControllerTest
  {
    private RecoverUsernameViewModel viewModel;
    private RedirectToRouteResultHelper resultHelper;
    private string emailTo;
    private string testEmail = "bad@email.com";

    public override void Given() {
      viewModel = new RecoverUsernameViewModel() { Email = testEmail };
      var user = new User("test", "test", "test", "user", "test");
      repository.Init(new List<User>() { user }.AsQueryable());
      emailService.Setup(s => s.SendSystemEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Callback<string, string, string>((to, subject, body) => emailTo = to);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.RecoverUsername(viewModel, true));
    }

    [Test]
    public void it_should_return_the_recover_username_complete_view() {
      resultHelper.ShouldRedirectTo("RecoverUsernameComplete");
    }

    [Test]
    public void it_should_send_the_user_an_email() {
      emailTo.Should().Be(testEmail);
    }
  }

  [TestFixture]
  public class with_an_invalid_view_model : UsersControllerTest
  {
    private RecoverUsernameViewModel viewModel;
    private ViewResultHelper<RecoverUsernameViewModel> resultHelper;

    public override void Given() {
      viewModel = new RecoverUsernameViewModel();
      controller.ModelState.AddModelError("Email", new Exception("Test"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<RecoverUsernameViewModel>(controller.RecoverUsername(viewModel, true));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeNullOrEmpty();
    }

    [Test]
    public void it_should_add_a_model_state_error_for_email() {
      resultHelper.Result.ViewData.ModelState.Keys.Should().Contain("Email");
    }
  }

  [TestFixture]
  public class with_an_invalid_captcha : UsersControllerTest
  {
    private RecoverUsernameViewModel viewModel;
    private ViewResultHelper<RecoverUsernameViewModel> resultHelper;

    public override void Given() {
      viewModel = new RecoverUsernameViewModel();
      viewModel.Email = "test@test.com";
    }

    public override void When() {
      resultHelper = new ViewResultHelper<RecoverUsernameViewModel>(controller.RecoverUsername(viewModel, false));
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
