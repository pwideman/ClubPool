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

namespace ClubPool.Tests.Controllers.Seasons.when_asked_for_the_detail_view
{
  [TestFixture]
  public class for_a_valid_season : SeasonsControllerTest
  {
    private ViewResultHelper<SeasonViewModel> resultHelper;
    private Season season;

    public override void Given() {
      season = new Season("test", GameType.EightBall);
      season.SetIdTo(1);

      repository.Setup(r => r.Get<Season>(season.Id)).Returns(season);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<SeasonViewModel>(controller.View(season.Id));
    }

    [Test]
    public void it_should_initialize_the_season_id() {
      resultHelper.Model.Id.Should().Be(season.Id);
    }

    [Test]
    public void it_should_initialize_the_season_name() {
      resultHelper.Model.Name.Should().Be(season.Name);
    }
  }

  [TestFixture]
  public class for_a_nonexistent_season : SeasonsControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.View(0));
    }

    [Test]
    public void it_should_return_an_http_not_found_result() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

}
