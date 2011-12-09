using System;
using System.Collections.Generic;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Seasons.when_asked_to_delete_a_season
{
  [TestFixture]
  public class that_can_be_deleted : SeasonsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private KeyValuePair<string, object> pageRouteValue;
    private int page = 2;
    private Season season;

    public override void Given() {
      season = new Season("Test", GameType.EightBall);
      season.SetIdTo(1);
      repository.Setup(r => r.Get<Season>(season.Id)).Returns(season);
      repository.Setup(r => r.Delete(season)).Verifiable();
      pageRouteValue = new KeyValuePair<string, object>("page", page);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Delete(season.Id, page));
    }

    [Test]
    public void it_should_delete_the_season() {
      repository.VerifyAll();
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_redirect_to_the_default_view() {
      resultHelper.ShouldRedirectTo("index");
    }

    [Test]
    public void it_should_redisplay_the_previous_page() {
      resultHelper.Result.RouteValues.Should().Contain(pageRouteValue);
    }
  }

  [TestFixture]
  public class that_is_nonexistant : SeasonsControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Delete(0, 0));
    }

    [Test]
    public void it_should_return_an_http_not_found_result() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class that_cannot_be_deleted : SeasonsControllerTest
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 1;
    static int page = 2;
    static KeyValuePair<string, object> pageRouteValue;

    public override void Given() {
      var season = new Season("name", GameType.EightBall);
      season.SetIdTo(id);
      season.IsActive = true; // will make CanDelete() return false
      repository.Setup(r => r.Get<Season>(season.Id)).Returns(season);
      pageRouteValue = new KeyValuePair<string, object>("page", page);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Delete(id, page));
    }

    [Test]
    public void it_should_return_an_error_message() {
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageErrorMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_redirect_to_the_default_view() {
      resultHelper.ShouldRedirectTo("index");
    }

    [Test]
    public void it_should_redisplay_the_previous_page() {
      resultHelper.Result.RouteValues.Should().Contain(pageRouteValue);
    }
  }

}
