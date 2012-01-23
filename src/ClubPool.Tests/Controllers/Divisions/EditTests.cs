using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Divisions;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Divisions.when_asked_for_the_edit_view
{
  [TestFixture]
  public class for_an_invalid_division : DivisionsControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Edit(0));
    }

    [Test]
    public void it_should_return_an_http_not_found_result() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class for_a_valid_division : DivisionsControllerTest
  {
    private ViewResultHelper<EditDivisionViewModel> resultHelper;
    private Division division;

    public override void Given() {
      division = new Division("temp", DateTime.Now, new Season("temp", GameType.EightBall));
      division.SetIdTo(1);
      repository.Setup(r => r.Get<Division>(division.Id)).Returns(division);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditDivisionViewModel>(controller.Edit(division.Id));
    }

    [Test]
    public void it_should_initialize_the_id_field() {
      resultHelper.Model.Id.Should().Be(division.Id);
    }

    [Test]
    public void it_should_initialize_the_name_field() {
      resultHelper.Model.Name.Should().Be(division.Name);
    }

    [Test]
    public void it_should_initialize_the_starting_date_field() {
      resultHelper.Model.StartingDate.Should().Be(division.StartingDate.ToShortDateString());
    }

    [Test]
    public void it_should_initialize_the_season_name_field() {
      resultHelper.Model.SeasonName.Should().Be(division.Season.Name);
    }
  }
}

namespace ClubPool.Tests.Controllers.Divisions.when_asked_to_edit_a_division
{
  [TestFixture]
  public class with_valid_data : DivisionsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditDivisionViewModel viewModel;
    private Division division;

    public override void Given() {
      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      season.SetVersionTo(1);

      division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(1);
      division.SetVersionTo(1);
      repository.Setup(r => r.Get<Division>(division.Id)).Returns(division);

      viewModel = CreateEditDivisionViewModel(division);
      viewModel.Name = "NewName";
      viewModel.StartingDate = "11/30/2010";
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_update_the_name() {
      division.Name.Should().Be(viewModel.Name);
    }

    [Test]
    public void it_should_update_the_starting_date() {
      division.StartingDate.ToShortDateString().Should().Be(viewModel.StartingDate);
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_redirect_to_the_view_season_view() {
      resultHelper.ShouldRedirectTo("view", "seasons");
    }
  }

  [TestFixture]
  public class with_an_invalid_id : DivisionsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditDivisionViewModel viewModel;

    public override void Given() {
      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      season.SetVersionTo(1);

      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(1);
      division.SetVersionTo(1);

      viewModel = CreateEditDivisionViewModel(division);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_index_view() {
      resultHelper.ShouldRedirectTo("index", "seasons");
    }

    [Test]
    public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class with_a_stale_version : DivisionsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditDivisionViewModel viewModel;

    public override void Given() {
      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      season.SetVersionTo(1);

      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(1);
      division.SetVersionTo(2);
      repository.Setup(r => r.Get<Division>(division.Id)).Returns(division);

      viewModel = CreateEditDivisionViewModel(division);
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(1);
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
  public class with_no_name : DivisionsControllerTest
  {
    private ViewResultHelper<EditDivisionViewModel> resultHelper;
    private EditDivisionViewModel viewModel;

    public override void Given() {
      viewModel = new EditDivisionViewModel();
      viewModel.Id = 1;
      viewModel.StartingDate = DateTime.Parse("1/1/2012").ToShortDateString();
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(1);
      controller.ModelState.AddModelError("Name", new Exception("Test"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditDivisionViewModel>(controller.Edit(viewModel));
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
    private ViewResultHelper<EditDivisionViewModel> resultHelper;
    private EditDivisionViewModel viewModel;

    public override void Given() {
      viewModel = new EditDivisionViewModel();
      viewModel.Id = 1;
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(1);
      viewModel.Name = "name";
      viewModel.StartingDate = "some bad date";
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditDivisionViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_is_related_to_the_date_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("StartingDate").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_data_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
    }
  }

  [TestFixture]
  public class with_a_duplicate_name : DivisionsControllerTest
  {
    private ViewResultHelper<EditDivisionViewModel> resultHelper;
    private EditDivisionViewModel viewModel;

    public override void Given() {
      var duplicateName = "MyDivision";

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      season.SetVersionTo(1);
      season.AddDivision(new Division(duplicateName, DateTime.Now, season));

      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(1);
      division.SetVersionTo(1);
      repository.Setup(r => r.Get<Division>(division.Id)).Returns(division);

      viewModel = CreateEditDivisionViewModel(division);
      viewModel.Name = duplicateName;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditDivisionViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_is_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_data_entered_by_the_user() {
      resultHelper.Model.Should().Be(viewModel);
    }
  }

}
