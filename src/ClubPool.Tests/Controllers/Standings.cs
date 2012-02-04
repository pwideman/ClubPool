using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Standings;
using ClubPool.Web.Controllers;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Standings
{
  public abstract class StandingsControllerTest : SpecificationContext
  {
    protected StandingsController controller;
    protected MockAuthenticationService authenticationService;
    protected Season season;
    protected IList<User> users;
    protected IList<Team> teams;
    protected IList<Meet> meets;
    protected User adminUser;

    public override void EstablishContext() {
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      var repository = new Mock<IRepository>();
      controller = new StandingsController(repository.Object, authenticationService);

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

namespace ClubPool.Tests.Controllers.Standings.when_asked_for_the_standings_view
{
  [TestFixture]
  public class and_there_is_no_acitve_season : StandingsControllerTest
  {
    private ViewResultHelper resultHelper;

    public override void Given() {
      season.IsActive = false;
    }

    public override void When() {
      resultHelper = new ViewResultHelper(controller.Standings());
    }

    [Test]
    public void it_should_return_the_error_view() {
      resultHelper.Result.ViewName.Should().Be("Error");
    }

    [Test]
    public void it_should_include_an_error_message() {
      resultHelper.Result.TempData.Should().ContainKey(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class and_the_active_season_has_no_divisions : StandingsControllerTest
  {
    private ViewResultHelper<SeasonStandingsViewModel> resultHelper;

    public override void Given() {
      season.Divisions.Clear();
    }

    public override void When() {
      resultHelper = new ViewResultHelper<SeasonStandingsViewModel>(controller.Standings());
    }

    [Test]
    public void it_should_return_the_view() {
      resultHelper.Result.Should().NotBeNull();
    }

    [Test]
    public void it_should_pass_the_model_to_the_view() {
      resultHelper.Result.Model.Should().BeOfType<SeasonStandingsViewModel>();
    }

    [Test]
    public void it_should_set_the_season_name() {
      resultHelper.Model.Name.Should().Be(season.Name);
    }

    [Test]
    public void it_should_indicate_that_the_season_has_no_divisions() {
      resultHelper.Model.HasDivisions.Should().BeFalse();
    }
  }

  [TestFixture]
  public class and_the_active_season_has_one_division_with_no_teams : StandingsControllerTest
  {
    private ViewResultHelper<SeasonStandingsViewModel> resultHelper;

    public override void Given() {
      foreach (var division in season.Divisions) {
        division.Teams.Clear();
      }
    }

    public override void When() {
      resultHelper = new ViewResultHelper<SeasonStandingsViewModel>(controller.Standings());
    }

    [Test]
    public void it_should_indicate_that_the_season_has_divisions() {
      resultHelper.Model.HasDivisions.Should().BeTrue();
    }

    [Test]
    public void it_should_include_division_standings() {
      resultHelper.Model.Divisions.Count().Should().Be(1);
    }

    [Test]
    public void it_should_set_the_division_names() {
      foreach(var division in season.Divisions) {
        resultHelper.Model.Divisions.Single(d => d.Id == division.Id).Name.Should().Be(division.Name);
      }
    }

    [Test]
    public void it_should_indicate_there_are_no_teams() {
      resultHelper.Model.Divisions.First().HasTeams.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_there_are_no_players() {
      resultHelper.Model.Divisions.First().HasPlayers.Should().BeFalse();
    }
  }

  [TestFixture]
  public class and_the_active_season_has_full_data : StandingsControllerTest
  {
    private ViewResultHelper<SeasonStandingsViewModel> resultHelper;

    public override void When() {
      resultHelper = new ViewResultHelper<SeasonStandingsViewModel>(controller.Standings());
    }

    [Test]
    public void it_should_indicate_there_are_teams() {
      resultHelper.Model.Divisions.First().HasTeams.Should().BeTrue();
    }

    [Test]
    public void it_should_indicate_there_are_players() {
      resultHelper.Model.Divisions.First().HasPlayers.Should().BeTrue();
    }

    [Test]
    public void it_should_indicate_there_are_12_teams_in_the_division() {
      resultHelper.Model.Divisions.First().Teams.Count().Should().Be(12);
    }

    [Test]
    public void it_should_indicate_there_are_24_players_in_the_division() {
      resultHelper.Model.Divisions.First().Players.Count().Should().Be(24);
    }

    [Test]
    public void it_should_indicate_there_are_24_players_overall() {
      resultHelper.Model.AllPlayers.Count().Should().Be(24);
    }
  
  }
}
