using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Testing;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Navigation;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Navigation.when_asked_for_the_menu
{
  public abstract class NavigationControllerTest : SpecificationContext
  {
    protected NavigationController controller;
    protected MockAuthenticationService authService;
    protected Mock<IRepository> repository;
    protected PartialViewResultHelper<MenuViewModel> resultHelper;

    public override void EstablishContext() {
      authService = AuthHelper.CreateMockAuthenticationService();
      repository = new Mock<IRepository>();
      controller = new NavigationController(authService, repository.Object);
      repository.Setup(r => r.All<Season>()).Returns(new List<Season>().AsQueryable());
    }

    public override void When() {
      resultHelper = new PartialViewResultHelper<MenuViewModel>(controller.Menu());
    }
  }

  [TestFixture]
  public class by_a_user_that_is_not_logged_in : NavigationControllerTest
  {
    public override void Given() {
      authService.MockPrincipal.MockIdentity.IsAuthenticated = false;
    }

    [Test]
    public void it_should_indicate_that_the_user_is_not_logged_in() {
      resultHelper.Model.UserIsLoggedIn.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_that_the_admin_menu_should_not_be_displayed() {
      resultHelper.Model.DisplayAdminMenu.Should().BeFalse();
    }
  }

  [TestFixture]
  public class by_a_normal_user : NavigationControllerTest
  {
    public override void Given() {
      var principal = authService.MockPrincipal;
      principal.MockIdentity.IsAuthenticated = true;
    }

    [Test]
    public void it_should_indicate_that_the_user_is_logged_in() {
      resultHelper.Model.UserIsLoggedIn.Should().BeTrue();
    }

    [Test]
    public void it_should_indicate_that_the_admin_menu_should_not_be_displayed() {
      resultHelper.Model.DisplayAdminMenu.Should().BeFalse();
    }
  }

  [TestFixture]
  public class by_an_admin_user : NavigationControllerTest
  {
    public override void Given() {
      var principal = authService.MockPrincipal;
      principal.MockIdentity.IsAuthenticated = true;
      principal.Roles = new string[] { Roles.Administrators };
    }

    [Test]
    public void it_should_indicate_that_the_user_is_logged_in() {
      resultHelper.Model.UserIsLoggedIn.Should().BeTrue();
    }

    [Test]
    public void it_should_indicate_that_the_admin_menu_should_be_displayed() {
      resultHelper.Model.DisplayAdminMenu.Should().BeTrue();
    }
  }

  [TestFixture]
  public class and_there_is_no_active_season : NavigationControllerTest
  {
    [Test]
    public void it_should_indicate_that_there_is_no_active_season() {
      resultHelper.Model.ActiveSeasonId.Should().Be(0);
    }
  }

  [TestFixture]
  public class and_there_is_an_active_season_and_user_has_team : NavigationControllerTest
  {
    private int id = 1;

    public override void Given() {
      var user = new User("Test", "test", "test", "test", "test");
      user.SetIdTo(id);
      authService.MockPrincipal.User = user;
      var season = new Season("Test", GameType.EightBall);
      season.SetIdTo(id);
      season.IsActive = true;
      var seasons = new List<Season>();
      seasons.Add(season);
      repository.Setup(r => r.All<Season>()).Returns(seasons.AsQueryable());
      var division = new Division("Test", DateTime.Now, season);
      var team = new Team("Test", division);
      team.SetIdTo(id);
      team.AddPlayer(user);
      repository.Init<Team>(new List<Team> { team }.AsQueryable());
    }

    [Test] public void it_should_return_has_active_season() {
      resultHelper.Model.HasActiveSeason.Should().BeTrue(); }

    [Test] public void it_should_return_the_active_season_id() {
      resultHelper.Model.ActiveSeasonId.Should().Be(id); }

    [Test] public void it_should_return_has_current_team() {
      resultHelper.Model.HasCurrentTeam.Should().BeTrue(); }

    [Test] public void it_should_return_the_current_team_id() {
      resultHelper.Model.CurrentTeamId.Should().Be(id); }
  }

}
