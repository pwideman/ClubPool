using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Divisions.ViewModels;
using ClubPool.Testing;

namespace ClubPool.Tests.Controllers.Divisions.when_asked_for_the_create_view
{
  [TestFixture]
  public class with_a_valid_season : DivisionsControllerTest
  {
    private ViewResultHelper<CreateDivisionViewModel> resultHelper;
    private int seasonId = 1;
    private string seasonName = "season1";
    private Season season;

    public override void Given() {
      season = new Season(seasonName, GameType.EightBall);
      season.SetIdTo(seasonId);
      repository.Setup(r => r.Get<Season>(seasonId)).Returns(season);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(seasonId));
    }

    [Test]
    public void it_should_initialize_the_season_id() {
      resultHelper.Model.SeasonId.Should().Be(seasonId);
    }

    [Test]
    public void it_should_initialize_the_season_name() {
      resultHelper.Model.SeasonName.Should().Be(seasonName);
    }
  }

  [TestFixture]
  public class with_an_invalid_season : DivisionsControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Create(0));
    }

    [Test]
    public void it_should_return_an_http_not_found_result() {
      resultHelper.Result.Should().NotBeNull();
    }
  }
}

namespace ClubPool.Tests.Controllers.Divisions.when_asked_to_create_a_division
{
  [TestFixture]
  public class with_valid_data : DivisionsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private CreateDivisionViewModel viewModel;
    private string name = "MyDivision";
    private DateTime startingDate = DateTime.Parse("11/30/2010");
    private int seasonId = 1;
    private Division savedDivision;

    public override void Given() {
      viewModel = new CreateDivisionViewModel();
      viewModel.Name = name;
      viewModel.StartingDate = startingDate.ToShortDateString();
      viewModel.SeasonId = seasonId;

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(seasonId);

      repository.Setup(r => r.Get<Season>(seasonId)).Returns(season);
      repository.Setup(r => r.All<Division>()).Returns(new List<Division>().AsQueryable());
      repository.Setup(r => r.SaveOrUpdate<Division>(It.IsAny<Division>())).Callback<Division>(d => savedDivision = d).Returns(savedDivision);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));
    }

    [Test]
    public void it_should_save_the_new_division() {
      savedDivision.Should().NotBeNull();
    }

    [Test]
    public void it_should_set_the_new_division_name() {
      savedDivision.Name.Should().Be(name);
    }

    [Test]
    public void it_should_set_the_new_division_starting_date() {
      savedDivision.StartingDate.Should().Be(startingDate);
    }

    [Test]
    public void it_should_redirect_to_the_view_season_view() {
      resultHelper.ShouldRedirectTo("seasons", "view");
    }
  }

  [TestFixture]
  public class with_an_invalid_season : DivisionsControllerTest
  {
    private ViewResultHelper<CreateDivisionViewModel> resultHelper;
    private CreateDivisionViewModel viewModel;

    public override void Given() {
      viewModel = new CreateDivisionViewModel();
      viewModel.Name = "MyDivision";
      viewModel.StartingDate = DateTime.Parse("11/30/2010").ToShortDateString();
      viewModel.SeasonId = 999;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_retain_the_data_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
    }

    [Test]
    public void it_should_not_save_the_division() {
      repository.Verify(r => r.SaveOrUpdate(It.IsAny<Division>()), Times.Never());
    }
  }

  [TestFixture]
  public class with_no_name : DivisionsControllerTest
  {
    private ViewResultHelper<CreateDivisionViewModel> resultHelper;
    private CreateDivisionViewModel viewModel;

    public override void Given() {
      viewModel = new CreateDivisionViewModel();
      viewModel.StartingDate = DateTime.Parse("11/30/2010").ToShortDateString();
      viewModel.SeasonId = 1;
    }

    public override void When() {
      controller.ModelState.AddModelError("Name", new Exception("Test"));
      resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_retain_the_data_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
    }
  }

  [TestFixture]
  public class with_an_invalid_starting_date : DivisionsControllerTest
  {
    private ViewResultHelper<CreateDivisionViewModel> resultHelper;
    private CreateDivisionViewModel viewModel;

    public override void Given() {
      viewModel = new CreateDivisionViewModel();
      viewModel.Name = "Test";
      viewModel.StartingDate = "a bad date string";
      viewModel.SeasonId = 1;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(viewModel));
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("StartingDate").Should().BeTrue();
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_retain_the_data_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
    }
  }

  [TestFixture]
  public class with_a_duplicate_name : DivisionsControllerTest
  {
    private ViewResultHelper<CreateDivisionViewModel> resultHelper;
    private CreateDivisionViewModel viewModel;
    private DateTime startingDate = DateTime.Parse("11/30/2010");
    private string name = "MyDivision";
    private int seasonId = 1;

    public override void Given() {
      viewModel = new CreateDivisionViewModel();
      viewModel.Name = name;
      viewModel.StartingDate = startingDate.ToShortDateString();
      viewModel.SeasonId = seasonId;

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(seasonId);
      season.AddDivision(new Division(name, DateTime.Now, season));
      repository.Setup(r => r.Get<Season>(seasonId)).Returns(season);

      var division = new Division(name, DateTime.Now, season);
      var divisions = new List<Division>();
      divisions.Add(division);
      repository.Setup(r => r.All<Division>()).Returns(divisions.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_retain_the_data_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
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
