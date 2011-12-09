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
  public class when_asked_for_the_edit_view : SeasonsControllerTest
  {
    private ViewResultHelper<EditSeasonViewModel> resultHelper;
    private int id = 1;
    private string name = "name";

    public override void Given() {
      var season = new Season(name, GameType.EightBall);
      repository.Setup(r => r.Get<Season>(id)).Returns(season);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditSeasonViewModel>(controller.Edit(id));
    }

    [Test]
    public void it_should_initialize_the_name_field() {
      resultHelper.Model.Name.Should().Be(name);
    }
  }
}

namespace ClubPool.Tests.Controllers.Seasons.when_asked_to_edit_a_season
{
  [TestFixture]
  public class with_valid_data : SeasonsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private string name = "name";
    private Season season;
    private EditSeasonViewModel viewModel;

    public override void Given() {
      season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      season.SetVersionTo(1);

      viewModel = new EditSeasonViewModel(season);
      viewModel.Name = name;

      repository.Setup(r => r.Get<Season>(season.Id)).Returns(season);
      repository.Init(new List<Season>() { season }.AsQueryable());
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_save_the_season() {
      repository.Verify(r => r.SaveChanges(), Times.Once());
    }

    [Test]
    public void it_should_update_the_season_name() {
      season.Name.Should().Be(name);
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
  public class with_invalid_data : SeasonsControllerTest
  {
    private ViewResultHelper<EditSeasonViewModel> resultHelper;

    public override void Given() {
      controller.ModelState.AddModelError("Name", new Exception("test"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditSeasonViewModel>(controller.Edit(new EditSeasonViewModel()));
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }
  }

  [TestFixture]
  public class with_an_invalid_id : SeasonsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditSeasonViewModel viewModel;

    public override void Given() {
      viewModel = new EditSeasonViewModel();
      viewModel.Id = 1;
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(1);
      viewModel.Name = "NewName";
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test] public void it_should_redirect_to_the_index_view() {
      resultHelper.ShouldRedirectTo("index"); }

    [Test] public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage); }
  }

  [TestFixture]
  public class with_a_stale_version : SeasonsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditSeasonViewModel viewModel;
    private int id = 1;
    private int version = 2;
    private string name = "name";
    private Season season;

    public override void Given() {
      season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      season.SetVersionTo(version);

      viewModel = new EditSeasonViewModel(season);
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(1);
      viewModel.Name = name;

      repository.Setup(r => r.Get<Season>(id)).Returns(season);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_edit_view() {
      resultHelper.ShouldRedirectTo("edit");
    }

    [Test]
    public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class with_a_duplicate_name : SeasonsControllerTest
  {
    private ViewResultHelper<EditSeasonViewModel> resultHelper;
    private EditSeasonViewModel viewModel;

    public override void Given() {
      var seasons = new List<Season>();
      for (int i = 1; i < 4; i++) {
        var season = new Season("season" + i.ToString(), GameType.EightBall);
        season.SetIdTo(i);
        season.SetVersionTo(1);
        seasons.Add(season);
      }
      viewModel = new EditSeasonViewModel(seasons[0]);
      viewModel.Name = seasons[1].Name;

      repository.Setup(r => r.Get<Season>(seasons[0].Id)).Returns(seasons[0]);
      repository.Init<Season>(seasons.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditSeasonViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }
  }

}
