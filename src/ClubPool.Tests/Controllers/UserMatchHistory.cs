using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.UserMatchHistory;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.UserMatchHistory
{
  public abstract class UserMatchHistoryControllerTest : SpecificationContext
  {
    protected UserMatchHistoryController controller;
    protected Season season;
    protected IList<User> users;
    protected IList<Team> teams;
    protected IList<Meet> meets;
    protected List<Web.Models.Match> matches;
    protected User adminUser;
    protected Mock<IRepository> repository;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      controller = new UserMatchHistoryController(repository.Object);

      teams = new List<Team>();
      users = new List<User>();
      var matchResults = new List<MatchResult>();
      matches = new List<Web.Models.Match>();
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

namespace ClubPool.Tests.Controllers.UserMatchHistory.when_asked_for_the_usermatchhistory_view
{
  [TestFixture]
  public class for_an_invalid_user : UserMatchHistoryControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.UserMatchHistory(0, null));
    }

    [Test]
    public void it_should_return_http_404_not_found() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class for_a_user_with_no_matches : UserMatchHistoryControllerTest
  {
    private ViewResultHelper<UserHistoryViewModel> resultHelper;
    private User user;

    public override void Given() {
      user = season.Divisions.First().Teams.First().Players.First();
      repository.Setup(r => r.Get<User>(user.Id)).Returns(user);
      repository.Setup(r => r.SqlQuery<ClubPool.Web.Models.Match>(It.IsAny<string>(), user.Id)).Returns(new List<ClubPool.Web.Models.Match>().AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<UserHistoryViewModel>(controller.UserMatchHistory(user.Id, null));
    }

    [Test]
    public void it_should_return_the_view() {
      resultHelper.Result.Should().NotBeNull();
    }

    [Test]
    public void it_should_pass_the_view_model_to_the_view() {
      resultHelper.Result.Model.Should().BeOfType<UserHistoryViewModel>();
    }

    [Test]
    public void it_should_set_the_users_name() {
      resultHelper.Model.Name.Should().Be(user.FullName);
    }

    [Test]
    public void it_should_indicate_that_there_are_no_matches() {
      resultHelper.Model.HasMatches.Should().BeFalse();
    }
  }

  [TestFixture]
  public class for_a_user_with_matches : UserMatchHistoryControllerTest
  {
    private ViewResultHelper<UserHistoryViewModel> resultHelper;
    private User user;

    public override void Given() {
      user = season.Divisions.First().Teams.First().Players.First();
      repository.Setup(r => r.Get<User>(user.Id)).Returns(user);
      var userMatches = from m in matches
                        from mp in m.Players
                        where mp.Player == user
                        select m;
      repository.Setup(r => r.SqlQuery<ClubPool.Web.Models.Match>(It.IsAny<string>(), user.Id)).Returns(userMatches.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<UserHistoryViewModel>(controller.UserMatchHistory(user.Id, null));
    }

    [Test]
    public void it_should_indicate_that_there_are_matches() {
      resultHelper.Model.HasMatches.Should().BeTrue();
    }
  }

}
