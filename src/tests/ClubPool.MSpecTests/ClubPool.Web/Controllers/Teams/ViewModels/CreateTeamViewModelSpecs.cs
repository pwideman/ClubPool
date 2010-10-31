using System;
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
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Teams;
using ClubPool.Web.Controllers.Teams.ViewModels;
using ClubPool.Framework.NHibernate;
using ClubPool.Framework.Extensions;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Teams.ViewModels
{
  public abstract class specification_for_CreateTeamViewModel
  {
    protected static CreateTeamViewModel viewModel;
    protected static IUserRepository userRepository;

    Establish context = () => {
      userRepository = MockRepository.GenerateStub<IUserRepository>();
    };

    protected static List<User> GetUsers(int count) {
      var users = new List<User>();
      for (var i = 1; i <= count; i++) {
        var user = new User("user" + i.ToString(), "pass", "first" + i.ToString(), "last" + i.ToString(), "user" + i.ToString() + "@email.com");
        user.SetIdTo(i);
        users.Add(user);
      }
      return users;
    }
  }

  [Subject(typeof(CreateTeamViewModel))]
  public class when_asked_for_new_empty_view_model : specification_for_CreateTeamViewModel
  {
    Because of = () => viewModel = new CreateTeamViewModel();

    It should_initialize_an_empty_Players_list = () =>
      viewModel.Players.Count().ShouldEqual(0);

    It should_initialize_an_empty_AvailablePlayers_list = () =>
      viewModel.AvailablePlayers.Count().ShouldEqual(0);
  }

  [Subject(typeof(CreateTeamViewModel))]
  public class when_asked_for_new_view_model : specification_for_CreateTeamViewModel
  {
    static Division division;
    static List<User> users;

    Establish context = () => {
      division = new Division("temp", DateTime.Now, new Season("temp", GameType.EightBall));
      division.SetIdTo(1);

      users = GetUsers(5);
      userRepository.Stub(r => r.GetUnassignedUsersForSeason(null)).IgnoreArguments().Return(users);
    };

    Because of = () => viewModel = new CreateTeamViewModel(userRepository, division);

    It should_initialize_the_division_id = () =>
      viewModel.DivisionId.ShouldEqual(division.Id);

    It should_initialize_the_division_name = () =>
      viewModel.DivisionName.ShouldEqual(division.Name);

    It should_initialize_an_empty_Players_list = () =>
      viewModel.Players.Count().ShouldEqual(0);

    It should_initialize_the_available_players_list = () =>
      users.Each(u => viewModel.AvailablePlayers.Where(p => p.Id == u.Id).Count().ShouldEqual(1));
  }
}