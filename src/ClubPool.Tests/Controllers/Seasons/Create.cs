using System;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Seasons;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Seasons.when_asked_to_create_a_season
{
  [TestFixture]
  public class with_valid_data : SeasonsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private CreateSeasonViewModel viewModel;
    private string name = "NewSeason";
    private Season savedSeason;

    public override void Given() {
      viewModel = new CreateSeasonViewModel();
      viewModel.Name = name;
      repository.Setup(r => r.SaveOrUpdate(It.IsAny<Season>())).Callback<Season>(s => savedSeason = s).Returns(savedSeason);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_default_view() {
      resultHelper.ShouldRedirectTo("index");
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_save_the_new_season() {
      savedSeason.Should().NotBeNull();
    }

    [Test]
    public void it_should_set_the_name_of_the_new_season() {
      savedSeason.Name.Should().Be(name);
    }
  }

  [TestFixture]
  public class with_invalid_data : SeasonsControllerTest
  {
    private ViewResultHelper<CreateSeasonViewModel> resultHelper;

    public override void Given() {
      controller.ModelState.AddModelError("Name", new Exception("test"));
    }
    public override void When() {
      resultHelper = new ViewResultHelper<CreateSeasonViewModel>(controller.Create(new CreateSeasonViewModel()));
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
