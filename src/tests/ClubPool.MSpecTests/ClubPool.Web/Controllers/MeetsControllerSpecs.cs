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
using ClubPool.Web.Controllers.Meets;
using ClubPool.Web.Controllers.Meets.ViewModels;
using ClubPool.Framework.NHibernate;
using ClubPool.Framework.Extensions;
using ClubPool.Testing;
using ClubPool.Testing.Core;
using ClubPool.Testing.ApplicationServices.Authentication;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_Meets_controller
  {
    protected static MeetsController controller;
    protected static IUserRepository userRepository;
    protected static MockAuthenticationService authenticationService;
    protected static IMeetRepository meetRepository;
    protected static int id = 1;
    protected static Meet meet;
    protected static User loggedInUser;

    Establish context = () => {
      userRepository = MockRepository.GenerateStub<IUserRepository>();
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      meetRepository = MockRepository.GenerateStub<IMeetRepository>();
      controller = new MeetsController(meetRepository, authenticationService, userRepository);
      ControllerHelper.CreateMockControllerContext(controller);

      loggedInUser = new User("test", "test", "test", "test", "test");
      userRepository.Stub(r => r.FindOne(null)).IgnoreArguments().WhenCalled(m => m.ReturnValue = loggedInUser);

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
      meetRepository.Stub(r => r.Get(id)).Return(meet);
    };
  }

  [Subject(typeof(MeetsController))]
  public class when_asked_for_the_view_action_for_an_invalid_meet : specification_for_Meets_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.View(0));

    It should_return_http_not_found = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(MeetsController))]
  public class when_asked_for_the_view_action_by_a_user_that_cannot_enter_results : specification_for_Meets_controller
  {
    static ViewResultHelper<MeetViewModel> resultHelper;

    Because of = () => resultHelper = new ViewResultHelper<MeetViewModel>(controller.View(id));

    It should_not_allow_the_user_to_enter_results = () =>
      resultHelper.Model.AllowUserToEnterResults.ShouldBeFalse();
  }

  [Subject(typeof(MeetsController))]
  public class when_asked_for_the_view_action_by_a_user_that_is_a_meet_participant : specification_for_Meets_controller
  {
    static ViewResultHelper<MeetViewModel> resultHelper;

    Establish context = () => {
      loggedInUser = meet.Teams.First().Players.First();
    };

    Because of = () => resultHelper = new ViewResultHelper<MeetViewModel>(controller.View(id));

    It should_allow_the_user_to_enter_results = () =>
      resultHelper.Model.AllowUserToEnterResults.ShouldBeTrue();
  }

  [Subject(typeof(MeetsController))]
  public class when_asked_for_the_view_action_by_a_user_that_is_an_admin : specification_for_Meets_controller
  {
    static ViewResultHelper<MeetViewModel> resultHelper;

    Establish context = () => {
      loggedInUser.AddRole(new Role(Roles.Administrators));
    };

    Because of = () => resultHelper = new ViewResultHelper<MeetViewModel>(controller.View(id));

    It should_allow_the_user_to_enter_results = () =>
      resultHelper.Model.AllowUserToEnterResults.ShouldBeTrue();
  }

  [Subject(typeof(MeetsController))]
  public class when_asked_for_the_view_action_by_a_user_that_is_an_officer : specification_for_Meets_controller
  {
    static ViewResultHelper<MeetViewModel> resultHelper;

    Establish context = () => {
      loggedInUser.AddRole(new Role(Roles.Officers));
    };

    Because of = () => resultHelper = new ViewResultHelper<MeetViewModel>(controller.View(id));

    It should_allow_the_user_to_enter_results = () =>
      resultHelper.Model.AllowUserToEnterResults.ShouldBeTrue();
  }
}
