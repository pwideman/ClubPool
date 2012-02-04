using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Dashboard;
using ClubPool.Web.Controllers.Dashboard.SidebarGadgets;
using ClubPool.Web.Controllers.Dashboard.ViewModels;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Dashboard
{
  public abstract class DashboardControllerTest : SpecificationContext
  {
    protected DashboardController controller;
    protected MockAuthenticationService authenticationService;
    protected Season season;
    protected IList<User> users;
    protected IList<Team> teams;
    protected IList<Meet> meets;
    protected User adminUser;

    public override void EstablishContext() {
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      var repository = new Mock<IRepository>();
      controller = new DashboardController(authenticationService, repository.Object);

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

namespace ClubPool.Tests.Controllers.Dashboard.when_asked_for_the_default_view
{
  [TestFixture]
  public class for_nonadmin_user_with_current_season_stats : DashboardControllerTest
  {
    private ViewResultHelper<DashboardViewModel> resultHelper;
    private User user;
    private int skillLevel;
    private User teammate;
    private Meet lastMeet;
    private Team team;
    private int seasonResultsCount;

    public override void Given() {
      user = users.Skip(2).First();
      authenticationService.MockPrincipal.User = user;
      skillLevel = user.SkillLevels.Where(sl => sl.GameType == season.GameType).First().Value;
      team = teams.Where(t => t.Players.Contains(user)).Single();
      teammate = team.Players.Where(p => p != user).Single();
      lastMeet = meets.Where(m => m.Teams.Contains(team) && m.IsComplete).OrderByDescending(m => m.Week).First();
      seasonResultsCount = (from m in meets
                            where m.Teams.Contains(team) && m.IsComplete
                            from match in m.Matches
                            where match.Players.Where(p => p.Player == user).Any()
                            select match).Count();
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DashboardViewModel>(controller.Dashboard(null));
    }

    [Test]
    public void it_should_indicate_that_the_user_is_not_an_administrator() {
      resultHelper.Model.UserIsAdmin.Should().BeFalse();
    }

    [Test]
    public void it_should_not_include_the_alerts_gadget() {
      resultHelper.SidebarGadgets.Keys.Should().NotContain(AlertsSidebarGadget.Name);
    }

    [Test]
    public void it_should_return_the_users_full_name() {
      resultHelper.Model.UserFullName.Should().Be(user.FullName);
    }

    [Test]
    public void it_should_indicate_that_there_are_current_season_stats() {
      resultHelper.Model.HasCurrentSeasonStats.Should().BeTrue();
    }

    [Test]
    public void it_should_return_the_current_season_stats() {
      resultHelper.Model.CurrentSeasonStats.Should().NotBeNull();
    }

    [Test]
    public void it_should_return_the_current_season_stats_skill_level() {
      resultHelper.Model.CurrentSeasonStats.SkillLevel.Should().Be(skillLevel);
    }

    [Test]
    public void it_should_return_the_current_season_stats_team_name() {
      resultHelper.Model.CurrentSeasonStats.TeamName.Should().Be(team.Name);
    }

    [Test]
    public void it_should_return_the_current_season_stats_team_id() {
      resultHelper.Model.CurrentSeasonStats.TeamId.Should().Be(team.Id);
    }

    [Test]
    public void it_should_return_the_current_season_stats_teammate_name() {
      resultHelper.Model.CurrentSeasonStats.TeammateName.Should().Be(teammate.FullName);
    }

    [Test]
    public void it_should_return_the_current_season_stats_teammate_id() {
      resultHelper.Model.CurrentSeasonStats.TeammateId.Should().Be(teammate.Id);
    }

    [Test]
    public void it_should_return_the_current_season_stats_personal_record() {
      resultHelper.Model.CurrentSeasonStats.PersonalRecord.Should().NotBeEmpty();
    }

    [Test]
    public void it_should_return_the_current_season_stats_team_record() {
      resultHelper.Model.CurrentSeasonStats.TeamRecord.Should().NotBeEmpty();
    }

    [Test]
    public void it_should_return_last_meet_stats() {
      resultHelper.Model.HasLastMeetStats.Should().BeTrue();
    }

    [Test]
    public void it_should_return_the_last_meet() {
      resultHelper.Model.LastMeetStats.OpponentTeam.Should().Be(lastMeet.Teams.Where(t => t != team).First().Name);
    }

    [Test]
    public void it_should_return_season_results() {
      resultHelper.Model.HasSeasonResults.Should().BeTrue();
    }

    [Test]
    public void it_should_return_the_season_results() {
      resultHelper.Model.SeasonResults.Count().Should().Be(seasonResultsCount);
    }

  }

  [TestFixture]
  public class for_admin_user_who_is_not_in_current_season : DashboardControllerTest
  {
    private ViewResultHelper<DashboardViewModel> resultHelper;

    public override void  Given() {
      authenticationService.MockPrincipal.User = adminUser;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DashboardViewModel>(controller.Dashboard(null));
    }

    [Test]
    public void it_should_indicate_that_the_user_is_an_administrator() {
      resultHelper.Model.UserIsAdmin.Should().BeTrue();
    }

    [Test]
    public void it_should_include_the_alerts_gadget() {
      resultHelper.SidebarGadgets.Keys.Should().Contain(AlertsSidebarGadget.Name);
    }

    [Test]
    public void it_should_not_return_current_season_stats() {
      resultHelper.Model.HasCurrentSeasonStats.Should().BeFalse();
    }

    [Test]
    public void it_should_not_return_last_meet_stats() {
      resultHelper.Model.HasLastMeetStats.Should().BeFalse();
    }
  }

  [TestFixture]
  public class for_a_different_user_by_nonadmin_user : DashboardControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.User = users.Skip(2).First();
    }

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Dashboard(users[0].Id));
    }

    [Test]
    public void it_should_return_http_not_found() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class for_a_different_user_by_admin_user : DashboardControllerTest
  {
    private ViewResultHelper<DashboardViewModel> resultHelper;
    private User user;

    public override void Given() {
      user = users.Skip(2).First();
      authenticationService.MockPrincipal.User = adminUser;
    }

    public override void When() {
      resultHelper = new ViewResultHelper<DashboardViewModel>(controller.Dashboard(user.Id));
    }

    [Test]
    public void it_should_return_the_view() {
      resultHelper.Result.Should().NotBeNull();
    }

    [Test]
    public void it_should_return_the_desired_user_information() {
      resultHelper.Model.UserFullName.Should().Be(user.FullName);
    }
  }

}
