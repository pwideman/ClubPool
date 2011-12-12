using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers;

namespace ClubPool.Tests.Controllers.Users
{
  [TestFixture]
  public class when_asked_to_delete_a_user : UsersControllerTest
  {
    static RedirectToRouteResultHelper resultHelper;
    static User user;
    static int page = 5;
    static KeyValuePair<string, object> pageRouteValue;
    static KeyValuePair<string, object> qRouteValue;

    public override void Given() {
      user = new User("test", "test", "test", "test", "test");
      user.SetIdTo(1);
      repository.Setup(r => r.Get<User>(user.Id)).Returns(user);
      repository.Setup(r => r.All<MatchResult>()).Returns(new List<MatchResult>().AsQueryable());
      pageRouteValue = new KeyValuePair<string, object>("page", page);
      qRouteValue = new KeyValuePair<string, object>("q", "test");
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Delete(user.Id, page, "test"));
    }

    [Test]
    public void it_should_delete_the_user() {
      repository.Verify(r => r.Delete(user), Times.Once());
    }

    [Test]
    public void it_should_redirect_to_the_index_view() {
      resultHelper.ShouldRedirectTo("index");
    }

    [Test]
    public void it_should_retain_the_current_page() {
      resultHelper.Result.RouteValues.Should().Contain(pageRouteValue);
    }

    [Test]
    public void it_should_retain_the_current_search_query() {
      resultHelper.Result.RouteValues.Should().Contain(qRouteValue);
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }
  }

  [TestFixture]
  public class when_asked_to_delete_an_invalid_user : UsersControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Delete(0, 5, null));
    }

    [Test]
    public void it_should_not_delete_the_user() {
      repository.Verify(r => r.Delete(It.IsAny<User>()), Times.Never());
    }

    [Test]
    public void it_should_return_an_http_not_found_result() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

}
