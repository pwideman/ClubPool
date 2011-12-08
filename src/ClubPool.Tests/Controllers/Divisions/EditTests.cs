using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers.Divisions.ViewModels;
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
