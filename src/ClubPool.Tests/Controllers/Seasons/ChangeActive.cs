using System;
using System.Linq;
using System.Collections.Generic;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Seasons.ViewModels;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Seasons
{
  [TestFixture]
  public class when_asked_for_the_change_active_season_view : SeasonsControllerTest
  {
    private ViewResultHelper<ChangeActiveViewModel> resultHelper;
    private string name = "active";
    private List<Season> seasons;

    public override void Given() {
      seasons = new List<Season>();

      var season = new Season(name, GameType.EightBall);
      season.IsActive = true;
      seasons.Add(season);

      for (int i = 1; i <= 5; i++) {
        season = new Season("other" + i.ToString(), GameType.EightBall);
        seasons.Add(season);
      }
      repository.Setup(r => r.All<Season>()).Returns(seasons.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<ChangeActiveViewModel>(controller.ChangeActive());
    }

    [Test]
    public void it_should_return_the_correct_current_active_season() {
      resultHelper.Model.CurrentActiveSeasonName.Should().Be(name);
    }

    [Test]
    public void it_should_return_the_inactive_seasons() {
      seasons.Where(s => !s.IsActive).Each(s => resultHelper.Model.InactiveSeasons.Where(inactive => inactive.Id == s.Id).Any().Should().BeTrue());
    }
  }
}

namespace ClubPool.Tests.Controllers.Seasons.when_asked_to_change_the_active_season
{
  [TestFixture]
  public class to_a_valid_season : SeasonsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private string name = "active";
    private List<Season> seasons;
    private int inactiveCount = 5;
    private int newActiveSeasonId = 10;
    private Season activeSeason;
    private Season newActiveSeason;

    public override void Given() {
      seasons = new List<Season>();

      activeSeason = new Season(name, GameType.EightBall);
      activeSeason.IsActive = true;
      seasons.Add(activeSeason);

      for (int i = 1; i <= inactiveCount; i++) {
        var season = new Season("other" + i.ToString(), GameType.EightBall);
        seasons.Add(season);
      }

      newActiveSeason = new Season("newactive", GameType.EightBall);
      newActiveSeason.SetIdTo(newActiveSeasonId);
      seasons.Add(newActiveSeason);

      repository.Setup(r => r.All<Season>()).Returns(seasons.AsQueryable());
      repository.Setup(r => r.SaveChanges()).Verifiable();
      repository.Setup(r => r.Get<Season>(newActiveSeasonId)).Returns(newActiveSeason);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.ChangeActive(newActiveSeasonId));
    }

    [Test]
    public void it_should_set_the_previous_active_season_to_inactive() {
      activeSeason.IsActive.Should().BeFalse();
    }

    [Test]
    public void it_should_set_the_new_active_season_to_active() {
      newActiveSeason.IsActive.Should().BeTrue();
    }

    [Test]
    public void it_should_save_the_new_active_season() {
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
  }

  [TestFixture]
  public class to_an_invalid_season : SeasonsControllerTest
  {
    private int badId = -1;
    private List<Season> seasons;
    private Season activeSeason;
    private HttpNotFoundResultHelper resultHelper;

    public override void Given() {
      activeSeason = new Season("name", GameType.EightBall);
      activeSeason.IsActive = true;
      seasons = new List<Season>();
      seasons.Add(activeSeason);

      repository.Init<Season>(seasons.AsQueryable());
    }

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.ChangeActive(badId));
    }

    [Test]
    public void it_should_not_change_the_active_season() {
      activeSeason.IsActive.Should().BeTrue();
    }

    [Test]
    public void it_should_not_save_anything() {
      repository.Verify(r => r.SaveChanges(), Times.Never());
    }

    [Test]
    public void it_should_return_an_http_not_found_result() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

}
