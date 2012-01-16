using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Meets;
using ClubPool.Web.Controllers.Meets.ViewModels;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Meets.when_asked_for_the_view_action
{
  public abstract class MeetsControllerTest : SpecificationContext
  {
    protected MeetsController controller;
    protected MockAuthenticationService authenticationService;
    protected Meet meet;
    protected User loggedInUser;
    protected ViewResultHelper<MeetViewModel> resultHelper;

    public override void EstablishContext() {
      var repository = new Mock<IRepository>();
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      controller = new MeetsController(repository.Object, authenticationService);

      loggedInUser = new User("test", "test", "test", "test", "test");
      loggedInUser.SetIdTo(99);
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
      authenticationService.MockPrincipal.MockIdentity.Name = loggedInUser.Username;

      // set up a meet
      var season = new Season("s1", GameType.EightBall);
      var division = new Division("d1", DateTime.Now, season);
      var team1 = new Team("team1", division);
      team1.AddPlayer(new User("t1p1", "pass", "a", "b", "e"));
      team1.AddPlayer(new User("t1p2", "pass", "c", "d", "e"));
      var team2 = new Team("team2", division);
      team2.AddPlayer(new User("t2p1", "pass", "e", "f", "e"));
      team2.AddPlayer(new User("t2p2", "pass", "e", "f", "e"));
      meet = new Meet(team1, team2, 0);
      meet.SetIdTo(1);
      repository.Setup(r => r.Get<Meet>(meet.Id)).Returns(meet);

      var users = team1.Players.Concat(team2.Players).Concat(new List<User> { loggedInUser });
      repository.Init<User>(users.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<MeetViewModel>(controller.View(meet.Id));
    }
  }

  [TestFixture]
  public class for_an_invalid_meet : MeetsControllerTest
  {
    private HttpNotFoundResultHelper notFoundResultHelper;

    public override void When() {
      notFoundResultHelper = new HttpNotFoundResultHelper(controller.View(0));
    }

    [Test]
    public void it_should_return_http_not_found() {
      notFoundResultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class by_a_user_that_cannot_enter_results : MeetsControllerTest
  {
    [Test]
    public void it_should_not_allow_the_user_to_enter_results() {
      resultHelper.Model.AllowUserToEnterResults.Should().BeFalse();
    }
  }

  [TestFixture]
  public class by_a_user_that_is_a_meet_participant : MeetsControllerTest
  {
    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.Name = meet.Teams.First().Players.First().Username;
    }

    [Test]
    public void it_should_allow_the_user_to_enter_results() {
      resultHelper.Model.AllowUserToEnterResults.Should().BeTrue();
    }
  }

  [TestFixture]
  public class by_a_user_that_is_an_admin : MeetsControllerTest
  {
    public override void Given() {
      loggedInUser.AddRole(new Role(Roles.Administrators));
    }

    [Test]
    public void it_should_allow_the_user_to_enter_results() {
      resultHelper.Model.AllowUserToEnterResults.Should().BeTrue();
    }
  }

  [TestFixture]
  public class by_a_user_that_is_an_officer : MeetsControllerTest
  {
    public override void Given() {
      loggedInUser.AddRole(new Role(Roles.Officers));
    }

    [Test]
    public void it_should_allow_the_user_to_enter_results() {
      resultHelper.Model.AllowUserToEnterResults.Should().BeTrue();
    }
  }

}
