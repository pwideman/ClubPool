using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Scoresheet;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Scoresheet
{
  public abstract class ScoresheetControllerTest : SpecificationContext
  {
    protected ScoresheetController controller;
    protected Season season;
    protected IList<User> users;
    protected IList<Team> teams;
    protected IList<Meet> meets;
    protected User adminUser;
    protected Mock<IRepository> repository;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      controller = new ScoresheetController(repository.Object);

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

namespace ClubPool.Tests.Controllers.Scoresheet.when_asked_for_the_schedule_view
{
  [TestFixture]
  public class for_an_invalid_meet : ScoresheetControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Scoresheet(0));
    }

    [Test]
    public void it_should_return_http_404_not_found() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class for_a_valid_meet : ScoresheetControllerTest
  {
    private ViewResultHelper<ScoresheetViewModel> resultHelper;
    private Meet meet;

    public override void Given() {
      meet = season.Divisions.First().Meets.First();
      repository.Setup(r => r.Get<Meet>(meet.Id)).Returns(meet);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<ScoresheetViewModel>(controller.Scoresheet(meet.Id));
    }

    [Test]
    public void it_should_return_the_view() {
      resultHelper.Result.Should().NotBeNull();
    }

    [Test]
    public void it_should_pass_the_model_to_the_view() {
      resultHelper.Result.Model.Should().BeOfType<ScoresheetViewModel>();
    }

    [Test]
    public void it_should_initialize_the_model_from_the_correct_meet() {
      resultHelper.Model.Id.Should().Be(meet.Id);
    }

    [Test]
    public void it_should_set_the_scheduled_week() {
      resultHelper.Model.ScheduledWeek.Should().Be(meet.Week + 1);
    }

    [Test]
    public void it_should_set_the_scheduled_date() {
      resultHelper.Model.ScheduledDate.Should().Be(meet.Division.StartingDate.AddDays(meet.Week * 7));
    }

    [Test]
    public void it_should_set_the_team_names() {
      var team1Name = resultHelper.Model.Team1Name;
      var team2Name = resultHelper.Model.Team2Name;
      foreach (var team in meet.Teams) {
        (team.Name.Equals(team1Name) || team.Name.Equals(team2Name)).Should().BeTrue();
      }
    }

    [Test]
    public void it_should_include_the_correct_number_of_matches() {
      resultHelper.Model.Matches.Count().Should().Be(4);
    }

  }
}
