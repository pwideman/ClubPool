using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Moq;
using FluentAssertions;
using NUnit.Framework;

using ClubPool.Testing;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers;

namespace ClubPool.Tests.Controllers.Divisions.when_asked_to_delete_a_division
{
  [TestFixture]
  public class that_does_not_exist : DivisionsControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Delete(0));
    }

    [Test]
    public void it_should_return_an_http_not_found_result() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class with_no_teams : DivisionsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private Division division;

    public override void Given() {
      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      division = new Division("temp", DateTime.Now, season);
      season.AddDivision(division);
      division.SetIdTo(1);
      repository.Setup(r => r.Get<Division>(division.Id)).Returns(division);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Delete(division.Id));
    }

    [Test]
    public void it_should_delete_the_division() {
      repository.Verify(r => r.Delete(division), Times.Once());
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_redirect_to_the_view_season_view() {
      resultHelper.ShouldRedirectTo("seasons", "view");
    }
  }

}
