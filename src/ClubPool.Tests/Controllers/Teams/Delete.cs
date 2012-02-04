using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Web.Models;
using ClubPool.Testing;
using ClubPool.Tests.Controllers.Teams;
using ClubPool.Web.Controllers;

namespace ClubPool.Tests.Controllers.Teams.when_asked_to_delete_a_team
{
  [TestFixture]
  public class that_does_not_exist : TeamsControllerTest
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
  public class with_no_players : TeamsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private int id = 1;
    private Team team;

    public override void Given() {
      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);
      team = new Team("temp", division);
      repository.Setup(r => r.Get<Team>(id)).Returns(team);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Delete(id));
    }

    [Test]
    public void it_should_delete_the_team() {
      repository.Verify(r => r.Delete(team));
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_redirect_to_the_view_season_view() {
      resultHelper.ShouldRedirectTo("details", "seasons");
    }
  }
}
