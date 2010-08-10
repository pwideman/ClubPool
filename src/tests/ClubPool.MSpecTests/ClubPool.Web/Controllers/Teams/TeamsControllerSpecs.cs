﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Net;

using Rhino.Mocks;
using Machine.Specifications;
using SharpArch.Testing;

using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.ApplicationServices.DomainManagement.Contracts;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Teams;
using ClubPool.Web.Controllers.Teams.ViewModels;
using ClubPool.Framework.NHibernate;
using ClubPool.Framework.Extensions;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Teams
{
  public abstract class specification_for_Teams_controller
  {
    protected static TeamsController controller;
    protected static IUserRepository userRepository;
    protected static IDivisionRepository divisionRepository;
    protected static ITeamRepository teamRepository;
    protected static ITeamManagementService teamService;

    Establish context = () => {
      userRepository = MockRepository.GenerateStub<IUserRepository>();
      divisionRepository = MockRepository.GenerateStub<IDivisionRepository>();
      teamRepository = MockRepository.GenerateStub<ITeamRepository>();
      teamService = MockRepository.GenerateStub<ITeamManagementService>();
      controller = new TeamsController(teamRepository, divisionRepository, userRepository, teamService);

      ControllerHelper.CreateMockControllerContext(controller);
      ServiceLocatorHelper.AddValidator();
    };
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_for_the_create_view : specification_for_Teams_controller
  {
    static ViewResultHelper<CreateTeamViewModel> resultHelper;
    static int divisionId = 1;
    static string divisionName = "MyDivision";
    static List<User> users;

    Establish context = () => {
      var division = new Division(divisionName, DateTime.Now);
      division.SetIdTo(divisionId);
      divisionRepository.Stub(r => r.Get(divisionId)).Return(division);

      users = DomainHelpers.GetUsers(5);
      userRepository.Stub(r => r.GetUnassignedUsersForSeason(null)).IgnoreArguments().Return(users);
    };

    Because of = () => resultHelper = new ViewResultHelper<CreateTeamViewModel>(controller.Create(divisionId));

    It should_initialize_the_division_id = () =>
      resultHelper.Model.DivisionId.ShouldEqual(divisionId);

    It should_initialize_the_division_name = () =>
      resultHelper.Model.DivisionName.ShouldEqual(divisionName);

    It should_initialize_the_available_players_list = () =>
      users.Each(u => resultHelper.Model.AvailablePlayers.Where(p => p.Id == u.Id).Count().ShouldEqual(1));

    It should_initialize_the_players_list = () =>
      resultHelper.Model.Players.Count().ShouldEqual(0);
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_for_the_create_view_with_an_invalid_division : specification_for_Teams_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Create(0));

    It should_return_an_http_not_found_result = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_to_create_a_team : specification_for_Teams_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static CreateTeamViewModel viewModel;
    static string name = "MyTeam";
    static int divisionId = 1;
    static Team savedTeam;
    static List<User> users;

    Establish context = () => {
      viewModel = new CreateTeamViewModel();
      viewModel.Name = name;
      viewModel.DivisionId = divisionId;

      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(divisionId);

      var season = new Season("temp");
      season.SetIdTo(1);
      division.Season = season;

      divisionRepository.Stub(r => r.Get(divisionId)).Return(division);
      teamService.Stub(s => s.TeamNameIsInUse(division, name)).IgnoreArguments().Return(false);
      teamRepository.Stub(r => r.SaveOrUpdate(null)).IgnoreArguments().Return(null).WhenCalled(m => savedTeam = m.Arguments[0] as Team);

      users = DomainHelpers.GetUsers(5);
      users.Each(u => userRepository.Stub(r => r.Get(u.Id)).Return(u));

      viewModel.Players = new List<PlayerViewModel>() { 
        new PlayerViewModel() { Id = users[0].Id },
        new PlayerViewModel() { Id = users[1].Id }
      };

    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));

    It should_save_the_new_team = () =>
      savedTeam.ShouldNotBeNull();

    It should_set_the_new_team_name = () =>
      savedTeam.Name.ShouldEqual(name);

    It should_set_the_new_team_players = () =>
      savedTeam.Players.Count().ShouldEqual(2);

    It should_redirect_to_the_view_season_view = () =>
      resultHelper.ShouldRedirectTo("seasons", "view");
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_to_create_a_team_with_model_errors : specification_for_Teams_controller
  {
    static ViewResultHelper<CreateTeamViewModel> resultHelper;
    static CreateTeamViewModel viewModel;
    static int divisionId = 1;
    static List<User> users;

    Establish context = () => {
      viewModel = new CreateTeamViewModel();
      viewModel.DivisionId = divisionId;

      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(1);
      var season = new Season("temp");
      season.SetIdTo(1);
      division.Season = season;

      divisionRepository.Stub(r => r.Get(divisionId)).Return(division);

      users = DomainHelpers.GetUsers(5);
      userRepository.Stub(r => r.GetUnassignedUsersForSeason(null)).IgnoreArguments().Return(users);
      users.Each(u => userRepository.Stub(r => r.Get(u.Id)).Return(u));
      viewModel.Players = new List<PlayerViewModel>() { new PlayerViewModel() { Id = users[0].Id } };
    };

    Because of = () => resultHelper = new ViewResultHelper<CreateTeamViewModel>(controller.Create(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_retain_the_players_set_by_the_user = () =>
      resultHelper.Model.Players.Where(p => p.Id == users[0].Id).Count().ShouldEqual(1);

    It should_remove_the_team_players_from_the_available_players_list = () =>
      resultHelper.Model.AvailablePlayers.Where(p => p.Id == users[0].Id).Any().ShouldBeFalse();

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_to_create_a_team_with_a_duplicate_name : specification_for_Teams_controller
  {
    static ViewResultHelper<CreateTeamViewModel> resultHelper;
    static CreateTeamViewModel viewModel;
    static int divisionId = 1;
    static List<User> users;
    static string name = "MyTeam";

    Establish context = () => {
      viewModel = new CreateTeamViewModel();
      viewModel.Name = name;
      viewModel.DivisionId = divisionId;

      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(1);
      var season = new Season("temp");
      season.SetIdTo(1);
      division.Season = season;

      divisionRepository.Stub(r => r.Get(divisionId)).Return(division);

      users = DomainHelpers.GetUsers(5);
      userRepository.Stub(r => r.GetUnassignedUsersForSeason(null)).IgnoreArguments().Return(users);
      users.Each(u => userRepository.Stub(r => r.Get(u.Id)).Return(u));
      viewModel.Players = new List<PlayerViewModel>() { new PlayerViewModel() { Id = users[0].Id } };

      teamService.Stub(s => s.TeamNameIsInUse(division, viewModel.Name)).Return(true);
    };

    Because of = () => resultHelper = new ViewResultHelper<CreateTeamViewModel>(controller.Create(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_retain_the_data_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_retain_the_players_set_by_the_user = () =>
      resultHelper.Model.Players.Where(p => p.Id == users[0].Id).Count().ShouldEqual(1);

    It should_remove_the_team_players_from_the_available_players_list = () =>
      resultHelper.Model.AvailablePlayers.Where(p => p.Id == users[0].Id).Any().ShouldBeFalse();

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();
  }


  [Subject(typeof(TeamsController))]
  public class when_asked_to_delete_an_invalid_team : specification_for_Teams_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Delete(0));

    It should_return_an_http_not_found_result = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_to_delete_a_team_that_contains_players : specification_for_Teams_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 1;

    Establish context = () => {
      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);
      var season = new Season("temp");
      season.SetIdTo(id);
      division.Season = season;
      var team = new Team("temp", division);
      team.AddPlayer(new User("temp", "pass", "first", "last", "email"));
      teamRepository.Stub(r => r.Get(id)).Return(team);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Delete(id));

    It should_return_an_error_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();

    It should_redirect_to_the_view_season_view = () =>
      resultHelper.ShouldRedirectTo("seasons", "view");
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_to_delete_a_team_with_no_players : specification_for_Teams_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 1;
    static Team team;

    Establish context = () => {
      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);
      var season = new Season("temp");
      season.SetIdTo(id);
      division.Season = season;
      team = new Team("temp", division);
      teamRepository.Stub(r => r.Get(id)).Return(team);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Delete(id));

    It should_delete_the_team = () =>
      teamRepository.AssertWasCalled(r => r.Delete(team));

    It should_return_a_notification_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_view_season_view = () =>
      resultHelper.ShouldRedirectTo("seasons", "view");
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_for_the_edit_view_for_invalid_team : specification_for_Teams_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Edit(0));

    It should_return_an_http_not_found_result = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_for_the_edit_view : specification_for_Teams_controller
  {
    static ViewResultHelper<EditTeamViewModel> resultHelper;
    static int id = 1;
    static Team team;
    static List<User> users;
    static int playerId = 10;

    Establish context = () => {
      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);
      division.Season = new Season("temp");

      users = DomainHelpers.GetUsers(5);
      userRepository.Stub(r => r.GetUnassignedUsersForSeason(null)).IgnoreArguments().Return(users);
      users.Each(u => userRepository.Stub(r => r.Get(u.Id)).Return(u));
      
      team = new Team("temp", division);
      team.SetIdTo(id);

      var player = new User("test", "pass", "first", "last", "email");
      player.SetIdTo(playerId);
      team.AddPlayer(player);
      teamRepository.Stub(r => r.Get(id)).Return(team);
    };

    Because of = () => resultHelper = new ViewResultHelper<EditTeamViewModel>(controller.Edit(id));

    It should_initialize_the_id_field = () =>
      resultHelper.Model.Id.ShouldEqual(team.Id);

    It should_initialize_the_name_field = () =>
      resultHelper.Model.Name.ShouldEqual(team.Name);

    It should_initialize_players_list = () =>
      resultHelper.Model.Players.Count().ShouldEqual(1);

    It should_initialize_available_players_list = () =>
      resultHelper.Model.AvailablePlayers.Count().ShouldEqual(5);
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_to_edit_a_team : specification_for_Teams_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static EditTeamViewModel viewModel;
    static string name = "MyTeam";
    static int id = 1;
    static Team team;
    static List<User> users;

    Establish context = () => {

      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);

      var season = new Season("temp");
      season.SetIdTo(id);
      division.Season = season;

      team = new Team("temp", division);
      team.SetIdTo(id);
      teamRepository.Stub(r => r.Get(id)).Return(team);

      users = DomainHelpers.GetUsers(5);
      userRepository.Stub(r => r.GetUnassignedUsersForSeason(null)).IgnoreArguments().Return(users);
      users.Each(u => userRepository.Stub(r => r.Get(u.Id)).Return(u));

      viewModel = new EditTeamViewModel(userRepository, team);
      viewModel.Name = name;
      viewModel.Players = new List<PlayerViewModel>() { 
        new PlayerViewModel() { Id = users[0].Id },
        new PlayerViewModel() { Id = users[1].Id }
      };

    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_update_the_name = () =>
      team.Name.ShouldEqual(name);

    It should_update_the_players = () =>
      team.Players.Select(p => p.Id).ToArray().ShouldEqual(viewModel.Players.Select(p => p.Id).ToArray());

    It should_return_a_notification_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_view_season_view = () =>
      resultHelper.ShouldRedirectTo("seasons", "view");
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_to_edit_a_team_with_a_model_error : specification_for_Teams_controller
  {
    static ViewResultHelper<EditTeamViewModel> resultHelper;
    static EditTeamViewModel viewModel;
    static int id = 1;
    static Team team;
    static List<User> users;

    Establish context = () => {

      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);

      var season = new Season("temp");
      season.SetIdTo(id);
      division.Season = season;

      team = new Team("temp", division);
      team.SetIdTo(id);
      var players = DomainHelpers.GetUsers(10, 2);
      players.Each(p => team.AddPlayer(p));
      teamRepository.Stub(r => r.Get(id)).Return(team);

      users = DomainHelpers.GetUsers(5);
      userRepository.Stub(r => r.GetUnassignedUsersForSeason(null)).IgnoreArguments().Return(users);
      users.Each(u => userRepository.Stub(r => r.Get(u.Id)).Return(u));

      viewModel = new EditTeamViewModel(userRepository, team);
      viewModel.Name = "";
      viewModel.Players = new List<PlayerViewModel>() { 
        new PlayerViewModel() { Id = users[0].Id },
        new PlayerViewModel() { Id = users[1].Id }
      };
    };

    Because of = () => resultHelper = new ViewResultHelper<EditTeamViewModel>(controller.Edit(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_is_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();

    It should_retain_the_players_set_by_the_user = () =>
      resultHelper.Model.Players.Select(p => p.Id).ToArray().ShouldEqual(viewModel.Players.Select(p => p.Id).ToArray());

    It should_add_the_teams_original_players_to_the_available_players_list = () =>
      team.Players.Each(p => resultHelper.Model.AvailablePlayers.Where(ap => ap.Id == p.Id).Any().ShouldBeTrue());

    It should_remove_the_new_team_players_from_the_available_players_list = () =>
      viewModel.Players.Each(p => resultHelper.Model.AvailablePlayers.Where(ap => ap.Id == p.Id).Any().ShouldBeFalse());

    It should_retain_the_still_available_players = () =>
      users.Where(u => !viewModel.Players.Select(p => p.Id).Contains(u.Id))
      .Each(u => resultHelper.Model.AvailablePlayers.Where(p => p.Id == u.Id).Any().ShouldBeTrue());
  }

  [Subject(typeof(TeamsController))]
  public class when_asked_to_edit_a_team_with_a_duplicate_name : specification_for_Teams_controller
  {
    static ViewResultHelper<EditTeamViewModel> resultHelper;
    static EditTeamViewModel viewModel;
    static int id = 1;
    static string name = "MyTeam";
    static Team team;
    static List<User> users;

    Establish context = () => {

      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);

      var season = new Season("temp");
      season.SetIdTo(id);
      division.Season = season;

      team = new Team("temp", division);
      team.SetIdTo(id);
      var players = DomainHelpers.GetUsers(10, 2);
      players.Each(p => team.AddPlayer(p));
      teamRepository.Stub(r => r.Get(id)).Return(team);

      users = DomainHelpers.GetUsers(5);
      userRepository.Stub(r => r.GetUnassignedUsersForSeason(null)).IgnoreArguments().Return(users);
      users.Each(u => userRepository.Stub(r => r.Get(u.Id)).Return(u));

      viewModel = new EditTeamViewModel(userRepository, team);
      viewModel.Name = name;
      viewModel.Players = new List<PlayerViewModel>() { 
        new PlayerViewModel() { Id = users[0].Id },
        new PlayerViewModel() { Id = users[1].Id }
      };

      teamService.Stub(s => s.TeamNameIsInUse(division, name)).Return(true);
    };

    Because of = () => resultHelper = new ViewResultHelper<EditTeamViewModel>(controller.Edit(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_is_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();

    It should_retain_the_players_set_by_the_user = () =>
      resultHelper.Model.Players.Select(p => p.Id).ToArray().ShouldEqual(viewModel.Players.Select(p => p.Id).ToArray());

    It should_add_the_teams_original_players_to_the_available_players_list = () =>
      team.Players.Each(p => resultHelper.Model.AvailablePlayers.Where(ap => ap.Id == p.Id).Any().ShouldBeTrue());

    It should_remove_the_new_team_players_from_the_available_players_list = () =>
      viewModel.Players.Each(p => resultHelper.Model.AvailablePlayers.Where(ap => ap.Id == p.Id).Any().ShouldBeFalse());

    It should_retain_the_still_available_players = () =>
      users.Where(u => !viewModel.Players.Select(p => p.Id).Contains(u.Id))
      .Each(u => resultHelper.Model.AvailablePlayers.Where(p => p.Id == u.Id).Any().ShouldBeTrue());
  }

}