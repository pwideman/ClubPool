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
using ClubPool.Web.Controllers.Matches;
using ClubPool.Web.Controllers.Matches.ViewModels;
using ClubPool.Framework.NHibernate;
using ClubPool.Framework.Extensions;
using ClubPool.Testing;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_Matches_controller
  {
    protected static MatchesController controller;
    protected static IMatchRepository matchRepository;
    protected static IUserRepository userRepository;
    protected static IMatchResultRepository matchResultRepository;

    Establish context = () => {
      matchRepository = MockRepository.GenerateStub<IMatchRepository>();
      userRepository = MockRepository.GenerateStub<IUserRepository>();
      matchResultRepository = MockRepository.GenerateStub<IMatchResultRepository>();
      controller = new MatchesController(matchRepository, userRepository, matchResultRepository);
      
      ControllerHelper.CreateMockControllerContext(controller);
      ServiceLocatorHelper.AddValidator();
    };
  }

  [Subject(typeof(MatchesController))]
  public class when_asked_to_edit_match_results_with_an_invalid_view_model : specification_for_Matches_controller
  {
    static JsonResultHelper<EditMatchResponseViewModel> resultHelper;
    static EditMatchViewModel viewModel;

    Establish context = () => {
      viewModel = new EditMatchViewModel() {
        Player1Innings = -1,
        Player1DefensiveShots = -1,
        Player1Wins = 10,
        Player2Innings = -1,
        Player2DefensiveShots = -1,
        Player2Wins = 10
      };
    };

    Because of = () => resultHelper = new JsonResultHelper<EditMatchResponseViewModel>(controller.Edit(viewModel));

    It should_return_success_false = () =>
      resultHelper.Data.Success.ShouldBeFalse();

    It should_return_a_validation_result_for_id = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Id").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_winner = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Winner").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player1_id = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player1Id").Count().ShouldEqual(1);
    
    It should_return_a_validation_result_for_player2_id = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player2Id").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player1_wins = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player1Wins").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player2_wins = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player2Wins").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player1_innings = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player1Innings").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player2_innings = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player2Innings").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player1_defensive_shots = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player1DefensiveShots").Count().ShouldEqual(1);

    It should_return_a_validation_result_for_player2_defensive_shots = () =>
      resultHelper.Data.ValidationResults.Where(r => r.PropertyName == "Player2DefensiveShots").Count().ShouldEqual(1);
  }


}
