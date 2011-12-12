using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Users.when_asked_for_the_View_view
{
  [TestFixture]
  public class for_an_invalid_user : UsersControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.View(0));
    }

    [Test]
    public void it_should_return_http_not_found() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class by_a_nonadmin_user : UsersControllerTest
  {
    private ViewResultHelper<ViewViewModel> resultHelper;
    private int id = 1;
    private string username = "test";

    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      repository.Setup(r => r.Get<User>(id)).Returns(new User(username, "test", "first", "last", "email"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<ViewViewModel>(controller.View(id));
    }

    [Test]
    public void it_should_return_the_correct_user() {
      resultHelper.Model.Username.Should().Be(username);
    }

    [Test]
    public void it_should_not_show_admin_properties() {
      resultHelper.Model.ShowAdminProperties.Should().BeFalse();
    }
  }

  [TestFixture]
  public class by_an_admin_user : UsersControllerTest
  {
    private ViewResultHelper<ViewViewModel> resultHelper;
    private int id = 1;
    private string username = "test";

    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      authenticationService.MockPrincipal.Roles = new string[] { Roles.Administrators };
      repository.Setup(r => r.Get<User>(id)).Returns(new User(username, "test", "first", "last", "email"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<ViewViewModel>(controller.View(id));
    }

    [Test]
    public void it_should_return_the_correct_user() {
      resultHelper.Model.Username.Should().Be(username);
    }

    [Test]
    public void it_should_show_admin_properties() {
      resultHelper.Model.ShowAdminProperties.Should().BeTrue();
    }
  }

}
