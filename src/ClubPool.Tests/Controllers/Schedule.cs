using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Schedule;
using ClubPool.Web.Controllers;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Schedule
{
  public abstract class ScheduleControllerTest : SpecificationContext
  {
    protected ScheduleController controller;
    protected MockAuthenticationService authenticationService;
    protected Season season;
    protected IList<User> users;
    protected IList<Team> teams;
    protected IList<Meet> meets;
    protected User adminUser;

    public override void EstablishContext() {
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      var repository = new Mock<IRepository>();
      controller = new ScheduleController(repository.Object, authenticationService);

      teams = new List<Team>();
      users = new List<User>();
      var matchResults = new List<MatchResult>();
      var matches = new List<Web.Models.Match>();
      meets = new List<Meet>();
      var divisions = new List<Division>();
      season = DomainModelHelper.CreateTestSeason(users, divisions, teams, meets, matches, matchResults);
      repository.InitAll(users.AsQueryable(), null, new List<Season> { season }.AsQueryable());
      foreach (var user in users) {
        user.UpdateSkillLevel(season.GameType, repository.Object);
      }
      adminUser = users[0];
    }
  }
}

namespace ClubPool.Tests.Controllers.Schedule.when_asked_for_the_schedule_view
{
  [TestFixture]
  public class and_there_is_no_current_season : ScheduleControllerTest
  {
    private ViewResultHelper resultHelper;

    public override void Given() {
      season.IsActive = false;
    }

    public override void When() {
      resultHelper = new ViewResultHelper(controller.Schedule());  
    }

    [Test]
    public void it_should_return_the_error_view() {
      resultHelper.Result.ViewName.Should().Be("Error");
    }

    [Test]
    public void it_should_add_a_page_error_message() {
      resultHelper.Result.TempData.Should().ContainKey(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class and_there_are_no_divisions_in_the_current_season : ScheduleControllerTest
  {
    private ViewResultHelper<SeasonScheduleViewModel> resultHelper;

    public override void Given() {
      season.Divisions.Clear();
    }

    public override void When() {
      resultHelper = new ViewResultHelper<SeasonScheduleViewModel>(controller.Schedule());
    }

    [Test]
    public void it_should_return_the_view() {
      resultHelper.Result.Should().NotBeNull();
    }

    [Test]
    public void it_should_pass_the_model_to_the_view() {
      resultHelper.Result.Model.Should().BeOfType<SeasonScheduleViewModel>();
    }

    [Test]
    public void it_should_set_the_season_name() {
      resultHelper.Model.Name.Should().Be(season.Name);
    }

    [Test]
    public void it_should_indicate_that_there_are_no_divisions() {
      resultHelper.Model.HasDivisions.Should().BeFalse();
    }
  }

  [TestFixture]
  public class and_there_are_no_meets_in_the_current_season : ScheduleControllerTest
  {
    private ViewResultHelper<SeasonScheduleViewModel> resultHelper;

    public override void Given() {
      foreach (var division in season.Divisions) {
        division.Meets.Clear();
      }
    }

    public override void When() {
      resultHelper = new ViewResultHelper<SeasonScheduleViewModel>(controller.Schedule());
    }

    [Test]
    public void it_should_return_the_view() {
      resultHelper.Result.Should().NotBeNull();
    }

    [Test]
    public void it_should_indicate_that_the_season_has_divisions() {
      resultHelper.Model.HasDivisions.Should().BeTrue();
    }

    [Test]
    public void it_should_show_all_divisions() {
      foreach (var division in season.Divisions) {
        resultHelper.Model.Divisions.Any(d => d.Id == division.Id).Should().BeTrue();
      }
    }

    [Test]
    public void it_should_show_all_division_names() {
      foreach (var division in season.Divisions) {
        resultHelper.Model.Divisions.Single(d => d.Id == division.Id).Name.Should().Be(division.Name);
      }
    }

    [Test]
    public void it_should_indicate_that_there_is_no_schedule_for_all_divisions() {
      foreach (var division in season.Divisions) {
        resultHelper.Model.Divisions.Single(d => d.Id == division.Id).HasSchedule.Should().BeFalse();
      }
    }
  }

  [TestFixture]
  public class and_there_are_meets_in_the_current_season : ScheduleControllerTest
  {
    private ViewResultHelper<SeasonScheduleViewModel> resultHelper;

    public override void When() {
      resultHelper = new ViewResultHelper<SeasonScheduleViewModel>(controller.Schedule());
    }

    [Test]
    public void it_should_return_the_view() {
      resultHelper.Result.Should().NotBeNull();
    }

    [Test]
    public void it_should_indicate_that_the_divisions_have_schedules() {
      foreach (var division in season.Divisions) {
        resultHelper.Model.Divisions.Single(d => d.Id == division.Id).HasSchedule.Should().BeTrue();
      }
    }

    [Test]
    public void it_should_include_a_schedule_for_each_division() {
      foreach (var division in season.Divisions) {
        resultHelper.Model.Divisions.Single(d => d.Id == division.Id).Schedule.Should().NotBeNull();
      }
    }

  }
}
