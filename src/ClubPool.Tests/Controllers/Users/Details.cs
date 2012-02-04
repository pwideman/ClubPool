using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Users;

namespace ClubPool.Tests.Controllers.Users.when_asked_for_the_details_view
{
  [TestFixture]
  public class for_an_invalid_user : UsersControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Details(0));
    }

    [Test]
    public void it_should_return_http_not_found() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class by_a_nonadmin_user : UsersControllerTest
  {
    private ViewResultHelper<DetailsViewModel> resultHelper;
    private int id = 1;
    private string username = "test";

    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      repository.Setup(r => r.Get<User>(id)).Returns(new User(username, "test", "first", "last", "email"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(id));
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
    private ViewResultHelper<DetailsViewModel> resultHelper;
    private int id = 1;
    private string username = "test";

    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      authenticationService.MockPrincipal.Roles = new string[] { Roles.Administrators };
      repository.Setup(r => r.Get<User>(id)).Returns(new User(username, "test", "first", "last", "email"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(id));
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
