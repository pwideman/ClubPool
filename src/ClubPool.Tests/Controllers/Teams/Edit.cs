using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Tests.Controllers.Teams;
using ClubPool.Testing;
using ClubPool.Web.Controllers.Teams.ViewModels;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers;

namespace ClubPool.Tests.Controllers.Teams.when_asked_for_the_edit_view
{
  [TestFixture]
  public class for_an_invalid_team : TeamsControllerTest
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
  public class for_a_valid_team : TeamsControllerTest
  {
    private ViewResultHelper<EditTeamViewModel> resultHelper;
    private int id = 1;
    private Team team;
    private List<User> users;
    private int playerId = 10;

    public override void Given() {
      var division = new Division("temp", DateTime.Now, new Season("temp", GameType.EightBall));
      division.SetIdTo(id);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), division.Season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      team = new Team("temp", division);
      team.SetIdTo(id);

      var player = new User("test", "pass", "first", "last", "email");
      player.SetIdTo(playerId);
      team.AddPlayer(player);
      repository.Setup(r => r.Get<Team>(id)).Returns(team);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditTeamViewModel>(controller.Edit(id));
    }

    [Test]
    public void it_should_initialize_the_id_field() {
      resultHelper.Model.Id.Should().Be(team.Id);
    }

    [Test]
    public void it_should_initialize_the_name_field() {
      resultHelper.Model.Name.Should().Be(team.Name);
    }

    [Test]
    public void it_should_initialize_players_list() {
      resultHelper.Model.Players.Count().Should().Be(6);
    }
  }
}

namespace ClubPool.Tests.Controllers.Teams.when_asked_to_edit_a_team
{
  [TestFixture]
  public class with_valid_data : TeamsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditTeamViewModel viewModel;
    private string name = "MyTeam";
    private int id = 1;
    private Team team;
    private int version = 1;
    private List<User> users;
    private Division division;
    private int schedulePriority = 99;

    public override void Given() {

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      team = new Team("Team 1", division);
      team.SetIdTo(id);
      team.SetVersionTo(version);
      repository.Setup(r => r.Get<Team>(id)).Returns(team);
      team.AddPlayer(users[0]);
      team.AddPlayer(users[1]);
      division.AddTeam(team);

      var team2 = new Team("Team 2", division);
      team2.SetIdTo(2);
      team.SetVersionTo(1);
      repository.Setup(r => r.Get<Team>(2)).Returns(team2);
      team2.AddPlayer(users[2]);
      team2.AddPlayer(users[3]);
      division.AddTeam(team2);

      division.CreateSchedule(repository.Object);

      viewModel = new EditTeamViewModel();
      viewModel.Id = id;
      viewModel.SchedulePriority = schedulePriority;
      viewModel.Name = name;
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(version);
      viewModel.SelectedPlayers = new int[2] { users[0].Id, users[4].Id };
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_update_the_name() {
      team.Name.Should().Be(name);
    }

    [Test]
    public void it_should_update_the_schedule_priority() {
      team.SchedulePriority.Should().Be(schedulePriority);
    }

    [Test]
    public void it_should_update_the_players() {
      team.Players.Select(p => p.Id).ToArray().Should().Contain(viewModel.SelectedPlayers);
    }

    [Test]
    public void it_should_remove_the_previous_player_from_the_teams_matches() {
      division.Meets.First().Matches.Where(match => match.Players.Where(p => p.Player == users[1]).Any()).Count().Should().Be(0);
    }

    [Test]
    public void it_should_add_the_new_player_to_the_teams_matches() {
      division.Meets.First().Matches.Where(match => match.Players.Where(p => p.Player == users[4]).Any()).Count().Should().Be(2);
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_redirect_to_the_view_season_view() {
      resultHelper.ShouldRedirectTo("view", "seasons");
    }
  }

  [TestFixture]
  public class with_valid_data_and_no_players_selected : TeamsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditTeamViewModel viewModel;
    private string name = "MyTeam";
    private int id = 1;
    private Team team;
    private int version = 1;
    private List<User> users;
    private Division division;
    private int schedulePriority = 99;

    public override void Given() {

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      team = new Team("Team 1", division);
      team.SetIdTo(id);
      team.SetVersionTo(version);
      repository.Setup(r => r.Get<Team>(id)).Returns(team);
      team.AddPlayer(users[0]);
      team.AddPlayer(users[1]);
      division.AddTeam(team);

      var team2 = new Team("Team 2", division);
      team2.SetIdTo(2);
      team.SetVersionTo(1);
      repository.Setup(r => r.Get<Team>(2)).Returns(team2);
      team2.AddPlayer(users[2]);
      team2.AddPlayer(users[3]);
      division.AddTeam(team2);

      division.CreateSchedule(repository.Object);

      viewModel = new EditTeamViewModel();
      viewModel.Id = id;
      viewModel.SchedulePriority = schedulePriority;
      viewModel.Name = name;
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(version);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_update_the_name() {
      team.Name.Should().Be(name);
    }

    [Test]
    public void it_should_update_the_schedule_priority() {
      team.SchedulePriority.Should().Be(schedulePriority);
    }

    [Test]
    public void it_should_remove_the_players() {
      team.Players.Should().BeEmpty();
    }

    [Test]
    public void it_should_remove_the_previous_players_from_the_teams_matches() {
      division.Meets.First().Matches.Should().BeEmpty();
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_redirect_to_the_view_season_view() {
      resultHelper.ShouldRedirectTo("view", "seasons");
    }
  }

  [TestFixture]
  public class with_an_invalid_id : TeamsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditTeamViewModel viewModel;
    private int id = 1;
    private int version = 1;
    private List<User> users;

    public override void Given() {

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);

      var team = new Team("temp", division);
      team.SetIdTo(id);
      team.SetVersionTo(version);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      viewModel = new EditTeamViewModel();
      viewModel.Id = 99;
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_seasons_index_view() {
      resultHelper.ShouldRedirectTo("index", "seasons");
    }

    [Test]
    public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class with_a_stale_version : TeamsControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private EditTeamViewModel viewModel;
    private int id = 1;
    private int version = 2;
    private List<User> users;

    public override void Given() {

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);

      var team = new Team("temp", division);
      team.SetIdTo(id);
      team.SetVersionTo(version);
      repository.Setup(r => r.Get<Team>(id)).Returns(team);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      viewModel = new EditTeamViewModel();
      viewModel.Id = team.Id;
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(1);
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_edit_view() {
      resultHelper.ShouldRedirectTo("edit");
    }

    [Test]
    public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

  [TestFixture]
  public class with_a_model_error : TeamsControllerTest
  {
    private ViewResultHelper<EditTeamViewModel> resultHelper;
    private EditTeamViewModel viewModel;
    private int id = 1;
    private int version = 1;
    private Team team;
    private List<User> users;

    public override void Given() {

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      season.SetVersionTo(version);

      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);
      division.SetVersionTo(version);

      team = new Team("temp", division);
      team.SetIdTo(id);
      team.SetVersionTo(version);

      var players = DomainModelHelper.GetUsers(10, 2);
      players.Each(p => team.AddPlayer(p));
      repository.Setup(r => r.Get<Team>(id)).Returns(team);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      viewModel = new EditTeamViewModel();
      viewModel.Id = team.Id;
      viewModel.Name = "";
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(version);
      viewModel.SelectedPlayers = new int[2] { users[0].Id, users[1].Id };

      controller.ModelState.AddModelError("Name", new Exception("test"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditTeamViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_is_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_player_selection_by_the_user() {
      viewModel.SelectedPlayers.Each(id => resultHelper.Model.Players.Single(p => p.Id == id).IsSelected.Should().BeTrue());
    }

    [Test]
    public void it_should_add_the_teams_current_players_to_the_players_list() {
      resultHelper.Model.Players.Select(p => p.Id).Should().Contain(team.Players.Select(p => p.Id));
    }
  }

  [TestFixture]
  public class with_a_model_error_and_no_players_selected : TeamsControllerTest
  {
    private ViewResultHelper<EditTeamViewModel> resultHelper;
    private EditTeamViewModel viewModel;
    private int id = 1;
    private int version = 1;
    private Team team;
    private List<User> users;

    public override void Given() {

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      season.SetVersionTo(version);

      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);
      division.SetVersionTo(version);

      team = new Team("temp", division);
      team.SetIdTo(id);
      team.SetVersionTo(version);

      var players = DomainModelHelper.GetUsers(10, 2);
      players.Each(p => team.AddPlayer(p));
      repository.Setup(r => r.Get<Team>(id)).Returns(team);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      viewModel = new EditTeamViewModel();
      viewModel.Id = team.Id;
      viewModel.Name = "";
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(version);

      controller.ModelState.AddModelError("Name", new Exception("test"));
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditTeamViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_is_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_player_selection_by_the_user() {
      resultHelper.Model.Players.Any(p => p.IsSelected).Should().BeFalse();
    }

    [Test]
    public void it_should_add_the_teams_current_players_to_the_players_list() {
      resultHelper.Model.Players.Select(p => p.Id).Should().Contain(team.Players.Select(p => p.Id));
    }
  }

  [TestFixture]
  public class with_a_duplicate_name : TeamsControllerTest
  {
    private ViewResultHelper<EditTeamViewModel> resultHelper;
    private EditTeamViewModel viewModel;
    private int id = 1;
    private int version = 1;
    private string name = "MyTeam";
    private Team team;
    private List<User> users;

    public override void Given() {

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);
      division.AddTeam(new Team(name, division));

      team = new Team("temp", division);
      team.SetIdTo(id);
      team.SetVersionTo(version);
      var players = DomainModelHelper.GetUsers(10, 2);
      players.Each(p => team.AddPlayer(p));
      repository.Setup(r => r.Get<Team>(id)).Returns(team);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      viewModel = new EditTeamViewModel();
      viewModel.Id = team.Id;
      viewModel.Name = name;
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(version);
      viewModel.SelectedPlayers = new int[2] { users[0].Id, users[1].Id };
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditTeamViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_is_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_player_selection_by_the_user() {
      viewModel.SelectedPlayers.Each(id => resultHelper.Model.Players.Single(p => p.Id == id).IsSelected.Should().BeTrue());
    }

    [Test]
    public void it_should_add_the_teams_current_players_to_the_players_list() {
      resultHelper.Model.Players.Select(p => p.Id).Should().Contain(team.Players.Select(p => p.Id));
    }
  }

  [TestFixture]
  public class with_a_duplicate_name_and_no_players_selected : TeamsControllerTest
  {
    private ViewResultHelper<EditTeamViewModel> resultHelper;
    private EditTeamViewModel viewModel;
    private int id = 1;
    private int version = 1;
    private string name = "MyTeam";
    private Team team;
    private List<User> users;

    public override void Given() {

      var season = new Season("temp", GameType.EightBall);
      season.SetIdTo(id);
      var division = new Division("temp", DateTime.Now, season);
      division.SetIdTo(id);
      division.AddTeam(new Team(name, division));

      team = new Team("temp", division);
      team.SetIdTo(id);
      team.SetVersionTo(version);
      var players = DomainModelHelper.GetUsers(10, 2);
      players.Each(p => team.AddPlayer(p));
      repository.Setup(r => r.Get<Team>(id)).Returns(team);

      users = DomainModelHelper.GetUsers(5);
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>(), season.Id)).Returns(users.AsQueryable());
      repository.Init<User>(users.AsQueryable(), true);

      viewModel = new EditTeamViewModel();
      viewModel.Id = team.Id;
      viewModel.Name = name;
      viewModel.Version = DomainModelHelper.ConvertIntVersionToString(version);
    }

    public override void When() {
      resultHelper = new ViewResultHelper<EditTeamViewModel>(controller.Edit(viewModel));
    }

    [Test]
    public void it_should_return_the_default_view() {
      resultHelper.Result.ViewName.Should().BeEmpty();
    }

    [Test]
    public void it_should_indicate_an_error() {
      resultHelper.Result.ViewData.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void it_should_indicate_the_error_is_related_to_the_name_field() {
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").Should().BeTrue();
    }

    [Test]
    public void it_should_retain_the_player_selection_by_the_user() {
      resultHelper.Model.Players.Any(p => p.IsSelected).Should().BeFalse();
    }

    [Test]
    public void it_should_add_the_teams_current_players_to_the_players_list() {
      resultHelper.Model.Players.Select(p => p.Id).Should().Contain(team.Players.Select(p => p.Id));
    }
  }

}
