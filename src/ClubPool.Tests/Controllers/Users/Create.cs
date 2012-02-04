using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Users;

namespace ClubPool.Tests.Controllers.Users
{
  // we don't have to test much with Create because it's the same code as SignUp.
  // technically our specs shouldn't know this and exercise them both, but...
  [TestFixture]
  public class when_asked_to_create_a_user : UsersControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private User user;
    private CreateViewModel viewModel;

    public override void Given() {
      viewModel = new CreateViewModel();
      viewModel.ConfirmPassword = "pass";
      viewModel.Password = "pass";
      viewModel.Username = "user";
      viewModel.FirstName = "user";
      viewModel.LastName = "user";
      viewModel.Email = "user@user.com";

      membershipService.Setup(s => s.CreateUser(viewModel.Username, viewModel.Password, viewModel.FirstName, viewModel.LastName, viewModel.Email, true, false))
        .Callback<string, string, string, string, string, bool, bool>((username, password, firstName, lastName, email, isApproved, isLocked) => {
          user = new User(username, password, firstName, lastName, email);
          user.IsApproved = isApproved;
          user.IsLocked = isLocked;
      }).Returns(() => user);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));
    }

    [Test]
    public void it_should_create_an_approved_user() {
      user.IsApproved.Should().BeTrue();
    }

    [Test]
    public void it_should_create_an_unlocked_user() {
      user.IsLocked.Should().BeFalse();
    }

    [Test]
    public void it_should_redirect_to_the_default_users_view() {
      resultHelper.ShouldRedirectTo("index");
    }
  }
}
