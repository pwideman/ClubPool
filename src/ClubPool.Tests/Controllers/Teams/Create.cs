using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Tests.Controllers.Teams;
using ClubPool.Web.Controllers.Teams;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Teams.when_asked_for_the_create_view
{
  [TestFixture]
  public class for_a_valid_division : TeamsControllerTest
  {
    private ViewResultHelper<CreateTeamViewModel> resultHelper;
    private int divisionId = 1;
    private string divisionName = "MyDivision";
    private List<User> users;

    public override void Given() {
      var division = new Division(divisionName, DateTime.Now, new Season("temp", GameType.EightBall));
      division.SetIdTo(divisionId);
      repository.Setup(r => r.Get<Division>(divisionId)).Returns(division);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), division.Season.Id)).Returns(users.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateTeamViewModel>(controller.Create(divisionId));
    }

    [Test]
    public void it_should_initialize_the_division_id() {
      resultHelper.Model.DivisionId.Should().Be(divisionId);
    }

    [Test]
    public void it_should_initialize_the_division_name() {
      resultHelper.Model.DivisionName.Should().Be(divisionName);
    }

    [Test]
    public void it_should_initialize_the_players_list() {
      users.Each(u => resultHelper.Model.Players.Where(p => p.Id == u.Id).Count().Should().Be(1));
    }
  }

  [TestFixture]
  public class with_an_invalid_division : TeamsControllerTest
  {
    private HttpNotFoundResultHelper resultHelper;

    public override void When() {
      resultHelper = new HttpNotFoundResultHelper(controller.Create(0));
    }

    [Test]
    public void it_should_return_an_http_not_found_result() {
      resultHelper.Result.Should().NotBeNull();
    }
  }

}

namespace ClubPool.Tests.Controllers.Teams.when_asked_to_create_a_team
{
  [TestFixture]
  public class with_valid_data : TeamsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private CreateTeamViewModel viewModel;
    private string name = "MyTeam";
    private int divisionId = 1;
    private Team savedTeam;
    private List<User> users;

    public override void Given() {
      viewModel = new CreateTeamViewModel();
      viewModel.Name = name;
      viewModel.DivisionId = divisionId;

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(divisionId);

      repository.Setup(r => r.Get<Division>(divisionId)).Returns(division);
      repository.Setup(r => r.SaveOrUpdate<Team>(It.IsAny<Team>())).Callback<Team>(t => savedTeam = t).Returns(savedTeam);

      users = DomainModelHelper.GetUsers(5);
      repository.Init<User>(users.AsQueryable(), true);

      viewModel.SelectedPlayers = new int[2] { users[0].Id, users[1].Id };
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));
    }

    [Test]
    public void it_should_save_the_new_team() {
      savedTeam.Should().NotBeNull();
    }

    [Test]
    public void it_should_set_the_new_team_name() {
      savedTeam.Name.Should().Be(name);
    }

    [Test]
    public void it_should_set_the_new_team_players() {
      savedTeam.Players.Count().Should().Be(2);
    }

    [Test]
    public void it_should_redirect_to_the_view_season_view() {
      resultHelper.ShouldRedirectTo("view", "seasons");
    }
  }

  [TestFixture]
  public class with_valid_data_and_no_players : TeamsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private CreateTeamViewModel viewModel;
    private string name = "MyTeam";
    private int divisionId = 1;
    private Team savedTeam;
    private List<User> users;

    public override void Given() {
      viewModel = new CreateTeamViewModel();
      viewModel.Name = name;
      viewModel.DivisionId = divisionId;

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(divisionId);

      repository.Setup(r => r.Get<Division>(divisionId)).Returns(division);
      repository.Setup(r => r.SaveOrUpdate<Team>(It.IsAny<Team>())).Callback<Team>(t => savedTeam = t).Returns(savedTeam);

      users = DomainModelHelper.GetUsers(5);
      repository.Init<User>(users.AsQueryable(), true);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));
    }

    [Test]
    public void it_should_save_the_new_team() {
      savedTeam.Should().NotBeNull();
    }

    [Test]
    public void it_should_set_the_new_team_name() {
      savedTeam.Name.Should().Be(name);
    }

    [Test]
    public void it_should_set_the_new_team_players() {
      savedTeam.Players.Should().BeEmpty();
    }

    [Test]
    public void it_should_redirect_to_the_view_season_view() {
      resultHelper.ShouldRedirectTo("view", "seasons");
    }
  }

  [TestFixture]
  public class with_model_errors : TeamsControllerTest
  {
    private ViewResultHelper<CreateTeamViewModel> resultHelper;
    private CreateTeamViewModel viewModel;
    private int divisionId = 1;
    private List<User> users;

    public override void Given() {
      viewModel = new CreateTeamViewModel();
      viewModel.DivisionId = divisionId;

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(1);

      repository.Setup(r => r.Get<Division>(divisionId)).Returns(division);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), division.Season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);
      viewModel.SelectedPlayers = new int[1] { users[0].Id };

      controller.ModelState.AddModelError("Name", new Exception("test"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateTeamViewModel>(controller.Create(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_retain_the_player_selection_by_the_user() {
      resultHelper.Model.Players.Single(p => p.Id == users[0].Id).IsSelected.Should().BeTrue();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }
  }

  [TestFixture]
  public class with_model_errors_and_no_players_selected : TeamsControllerTest
  {
    private ViewResultHelper<CreateTeamViewModel> resultHelper;
    private CreateTeamViewModel viewModel;
    private int divisionId = 1;
    private List<User> users;

    public override void Given() {
      viewModel = new CreateTeamViewModel();
      viewModel.DivisionId = divisionId;

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(1);

      repository.Setup(r => r.Get<Division>(divisionId)).Returns(division);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), division.Season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      controller.ModelState.AddModelError("Name", new Exception("test"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateTeamViewModel>(controller.Create(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_retain_the_player_selection_by_the_user() {
      resultHelper.Model.Players.Any(p => p.IsSelected).Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }
  }

  [TestFixture]
  public class with_a_duplicate_name : TeamsControllerTest
  {
    private ViewResultHelper<CreateTeamViewModel> resultHelper;
    private CreateTeamViewModel viewModel;
    private int divisionId = 1;
    private List<User> users;
    private string name = "MyTeam";

    public override void Given() {
      viewModel = new CreateTeamViewModel();
      viewModel.Name = name;
      viewModel.DivisionId = divisionId;

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(1);
      division.AddTeam(new Team(name, division));
      repository.Setup(r => r.Get<Division>(divisionId)).Returns(division);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);
      viewModel.SelectedPlayers = new int[1] { users[0].Id };
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateTeamViewModel>(controller.Create(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_retain_the_team_name_entered_by_the_user() {
      resultHelper.Model.Name.Should().Be(viewModel.Name);
    }

    [Test]
    public void it_should_retain_the_schedule_priority_entered_by_the_user() {
      resultHelper.Model.SchedulePriority.Should().Be(viewModel.SchedulePriority);
    }

    [Test]
    public void it_should_retain_the_player_selection_by_the_user() {
      resultHelper.Model.Players.Single(p => p.Id == users[0].Id).IsSelected.Should().BeTrue();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }
  }


  [TestFixture]
  public class with_a_duplicate_name_and_no_players_selected : TeamsControllerTest
  {
    private ViewResultHelper<CreateTeamViewModel> resultHelper;
    private CreateTeamViewModel viewModel;
    private int divisionId = 1;
    private List<User> users;
    private string name = "MyTeam";

    public override void Given() {
      viewModel = new CreateTeamViewModel();
      viewModel.Name = name;
      viewModel.DivisionId = divisionId;

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(1);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(1);
      division.AddTeam(new Team(name, division));
      repository.Setup(r => r.Get<Division>(divisionId)).Returns(division);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<CreateTeamViewModel>(controller.Create(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_retain_the_team_name_entered_by_the_user() {
      resultHelper.Model.Name.Should().Be(viewModel.Name);
    }

    [Test]
    public void it_should_retain_the_schedule_priority_entered_by_the_user() {
      resultHelper.Model.SchedulePriority.Should().Be(viewModel.SchedulePriority);
    }

    [Test]
    public void it_should_retain_the_player_selection_by_the_user() {
      resultHelper.Model.Players.Any(p => p.IsSelected).Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_was_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }
  }
}
