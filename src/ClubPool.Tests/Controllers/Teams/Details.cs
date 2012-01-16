using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Teams.ViewModels;

namespace ClubPool.Tests.Controllers.Teams
{
  public class DetailsTest : TeamsControllerTest
  {
    protected Season season;
    protected IList<Division> divisions;
    protected IList<Team> teams;
    protected IList<Meet> meets;
    protected IList<ClubPool.Web.Models.Match> matches;
    protected IList<MatchResult> matchResults;
    protected IList<User> users;
    protected User adminUser;
    protected User officerUser;
    protected Team team;

    public override void EstablishContext() {
      base.EstablishContext();
      users = new List<User>();
      divisions = new List<Division>();
      teams = new List<Team>();
      matches = new List<ClubPool.Web.Models.Match>();
      meets = new List<Meet>();
      matchResults = new List<MatchResult>();
      season = DomainModelHelper.CreateTestSeason(users, divisions, teams, meets, matches, matchResults);
      adminUser = users[0];
      officerUser = users[1];
      repository.Init<Division>(divisions.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);
      repository.Init<Team>(teams.AsQueryable(), true);
      repository.Init<MatchResult>(matchResults.AsQueryable());

      foreach (var user in users) {
        user.UpdateSkillLevel(season.GameType, repository.Object);
      }
      team = teams[0];
    }
  }
}

namespace ClubPool.Tests.Controllers.Teams.when_asked_for_the_details_view
{
  [TestFixture]
  public class by_an_administrator : DetailsTest
  {
    private int numberOfSeasonResults;
    private ViewResultHelper<DetailsViewModel> resultHelper;

    public override void Given() {
      authService.MockPrincipal.User = adminUser;
      numberOfSeasonResults = meets.Where(m => m.Teams.Contains(team) && m.Matches.Where(match => match.IsComplete).Any()).Count();
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));
    }

    [Test]
    public void it_should_return_the_team_name() {
      resultHelper.Model.Name.Should().Be(team.Name);
    }

    [Test]
    public void it_should_allow_the_user_to_update_the_team_name() {
      resultHelper.Model.CanUpdateName.Should().BeTrue();
    }

    [Test]
    public void it_should_return_the_correct_number_of_players() {
      resultHelper.Model.Players.Count().Should().Be(team.Players.Count());
    }

    [Test]
    public void it_should_return_the_correct_players() {
      resultHelper.Model.Players.Select(p => p.Name).Each(name => team.Players.Where(p => p.FullName.Equals(name)).Any().Should().BeTrue());
    }

    [Test]
    public void it_should_return_the_correct_player_skill_levels() {
      resultHelper.Model.Players.Each(player => player.EightBallSkillLevel.Should().Be(
        team.Players.Where(p => p.FullName.Equals(player.Name)).Single()
        .SkillLevels.Where(sl => sl.GameType == team.Division.Season.GameType).First().Value));
    }

    [Test]
    public void it_should_return_has_season_results() {
      resultHelper.Model.HasSeasonResults.Should().BeTrue();
    }

    [Test]
    public void it_should_return_the_correct_number_of_season_results() {
      resultHelper.Model.SeasonResults.Count().Should().Be(numberOfSeasonResults);
    }
  }

  [TestFixture]
  public class by_an_officer : DetailsTest
  {
    private ViewResultHelper<DetailsViewModel> resultHelper;

    public override void Given() {
      authService.MockPrincipal.User = officerUser;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));
    }

    [Test]
    public void it_should_allow_the_user_to_update_the_team_name() {
      resultHelper.Model.CanUpdateName.Should().BeTrue();
    }
  }

  [TestFixture]
  public class by_a_team_member : DetailsTest
  {
    private ViewResultHelper<DetailsViewModel> resultHelper;

    public override void Given() {
      var user = team.Players.First();
      authService.MockPrincipal.User = user;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));
    }

    [Test]
    public void it_should_allow_the_user_to_update_the_team_name() {
      resultHelper.Model.CanUpdateName.Should().BeTrue();
    }
  }

  [TestFixture]
  public class by_a_normal_user_not_on_this_team : DetailsTest
  {
    private ViewResultHelper<DetailsViewModel> resultHelper;

    public override void Given() {
      var user = teams[1].Players.First();
      authService.MockPrincipal.User = user;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DetailsViewModel>(controller.Details(team.Id));
    }

    [Test]
    public void it_should_not_allow_the_user_to_update_the_team_name() {
      resultHelper.Model.CanUpdateName.Should().BeFalse();
    }
  }

  [TestFixture]
  public class for_a_nonexistent_team : DetailsTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void Given() {
      authService.MockPrincipal.User = adminUser;
    }

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Details(9999));
    }

    [Test]
    public void it_should_return_http_not_found() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

}
