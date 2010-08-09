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
using ClubPool.Web.Controllers.Divisions;
using ClubPool.Web.Controllers.Divisions.ViewModels;
using ClubPool.Framework.NHibernate;
using ClubPool.Framework.Extensions;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public abstract class specification_for_Divisions_controller
  {
    protected static DivisionsController controller;
    protected static ISeasonRepository seasonsRepository;
    protected static IDivisionRepository divisionsRepository;

    Establish context = () => {
      seasonsRepository = MockRepository.GenerateStub<ISeasonRepository>();
      divisionsRepository = MockRepository.GenerateStub<IDivisionRepository>();
      controller = new DivisionsController(divisionsRepository, seasonsRepository);

      ControllerHelper.CreateMockControllerContext(controller);
      ServiceLocatorHelper.AddValidator();
    };
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_for_the_create_view : specification_for_Divisions_controller
  {
    static ViewResultHelper<CreateDivisionViewModel> resultHelper;
    static int seasonId = 1;
    static string seasonName = "season1";
    static Season season;

    Establish context = () => {
      season = new Season(seasonName);
      season.SetIdTo(seasonId);
      seasonsRepository.Stub(r => r.Get(seasonId)).Return(season);
    };

    Because of = () => resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(seasonId));

    It should_initialize_the_season_id = () =>
      resultHelper.Model.SeasonId.ShouldEqual(seasonId);

    It should_initialize_the_season_name = () =>
      resultHelper.Model.SeasonName.ShouldEqual(seasonName);
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_for_the_create_view_with_an_invalid_season : specification_for_Divisions_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Create(0));

    It should_return_an_http_not_found_result = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_create_a_division : specification_for_Divisions_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static CreateDivisionViewModel viewModel;
    static string name = "MyDivision";
    static DateTime startingDate = DateTime.Parse("11/30/2010");
    static int seasonId = 1;
    static Division savedDivision;

    Establish context = () => {
      viewModel = new CreateDivisionViewModel();
      viewModel.Name = name;
      viewModel.StartingDate = startingDate.ToShortDateString();
      viewModel.SeasonId = seasonId;

      var season = new Season("temp");
      season.SetIdTo(seasonId);

      seasonsRepository.Stub(r => r.Get(seasonId)).Return(season);
      divisionsRepository.Stub(r => r.GetAll()).Return(new List<Division>().AsQueryable());
      divisionsRepository.Stub(r => r.SaveOrUpdate(null)).IgnoreArguments().Return(null).WhenCalled(m => savedDivision = m.Arguments[0] as Division);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));

    It should_save_the_new_division = () =>
      savedDivision.ShouldNotBeNull();

    It should_set_the_new_division_name = () =>
      savedDivision.Name.ShouldEqual(name);

    It should_set_the_new_division_starting_date = () =>
      savedDivision.StartingDate.ShouldEqual(startingDate);

    It should_redirect_to_the_view_season_view = () =>
      resultHelper.ShouldRedirectTo("seasons", "view");
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_create_a_division_with_invalid_season : specification_for_Divisions_controller
  {
    static ViewResultHelper<CreateDivisionViewModel> resultHelper;
    static CreateDivisionViewModel viewModel;
    static string name = "MyDivision";
    static DateTime startingDate = DateTime.Parse("11/30/2010");
    static int seasonId = 1;

    Establish context = () => {
      viewModel = new CreateDivisionViewModel();
      viewModel.Name = name;
      viewModel.StartingDate = startingDate.ToShortDateString();
      viewModel.SeasonId = seasonId;
    };

    Because of = () => resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_retain_the_data_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_not_save_the_division = () =>
      divisionsRepository.AssertWasNotCalled(r => r.SaveOrUpdate(null), x => x.IgnoreArguments());      
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_create_a_division_with_model_errors : specification_for_Divisions_controller
  {
    static ViewResultHelper<CreateDivisionViewModel> resultHelper;
    static CreateDivisionViewModel viewModel;
    static DateTime startingDate = DateTime.Parse("11/30/2010");
    static int seasonId = 1;

    Establish context = () => {
      viewModel = new CreateDivisionViewModel();
      viewModel.StartingDate = startingDate.ToShortDateString();
      viewModel.SeasonId = seasonId;
    };

    Because of = () => resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_retain_the_data_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_create_a_division_with_a_duplicate_name : specification_for_Divisions_controller
  {
    static ViewResultHelper<CreateDivisionViewModel> resultHelper;
    static CreateDivisionViewModel viewModel;
    static DateTime startingDate = DateTime.Parse("11/30/2010");
    static string name = "MyDivision";
    static int seasonId = 1;

    Establish context = () => {
      viewModel = new CreateDivisionViewModel();
      viewModel.Name = name;
      viewModel.StartingDate = startingDate.ToShortDateString();
      viewModel.SeasonId = seasonId;

      var season = new Season("temp");
      season.SetIdTo(seasonId);
      seasonsRepository.Stub(r => r.Get(seasonId)).Return(season);

      var division = new Division(name, DateTime.Now);
      division.Season = season;
      var divisions = new List<Division>();
      divisions.Add(division);
      divisionsRepository.Stub(r => r.GetAll()).Return(divisions.AsQueryable());
    };

    Because of = () => resultHelper = new ViewResultHelper<CreateDivisionViewModel>(controller.Create(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_retain_the_data_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_delete_an_invalid_division : specification_for_Divisions_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Delete(0));

    It should_return_an_http_not_found_result = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_delete_a_division_that_contains_teams : specification_for_Divisions_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 1;

    Establish context = () => {
      var division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);
      var season = new Season("temp");
      season.SetIdTo(1);
      division.Season = season;
      division.AddTeam(new Team("temp", division));
      divisionsRepository.Stub(r => r.Get(id)).Return(division);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Delete(id));

    It should_return_an_error_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();

    It should_redirect_to_the_view_season_view = () =>
      resultHelper.ShouldRedirectTo("seasons", "view");
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_delete_a_division_with_no_teams : specification_for_Divisions_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 1;
    static Division division;

    Establish context = () => {
      division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);
      var season = new Season("temp");
      season.SetIdTo(1);
      division.Season = season;
      divisionsRepository.Stub(r => r.Get(id)).Return(division);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Delete(id));

    It should_delete_the_division = () =>
      divisionsRepository.AssertWasCalled(r => r.Delete(division));

    It should_return_a_notification_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_view_season_view = () =>
      resultHelper.ShouldRedirectTo("seasons", "view");
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_for_the_edit_view_for_invalid_division : specification_for_Divisions_controller
  {
    static HttpNotFoundResultHelper resultHelper;

    Because of = () => resultHelper = new HttpNotFoundResultHelper(controller.Edit(0));

    It should_return_an_http_not_found_result = () =>
      resultHelper.Result.ShouldNotBeNull();
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_for_the_edit_view : specification_for_Divisions_controller
  {
    static ViewResultHelper<EditDivisionViewModel> resultHelper;
    static int id = 1;
    static Division division;

    Establish context = () => {
      division = new Division("temp", DateTime.Now);
      division.SetIdTo(id);
      division.Season = new Season("temp");
      divisionsRepository.Stub(r => r.Get(id)).Return(division);
    };

    Because of = () => resultHelper = new ViewResultHelper<EditDivisionViewModel>(controller.Edit(id));

    It should_initialize_the_id_field = () =>
      resultHelper.Model.Id.ShouldEqual(division.Id);

    It should_initialize_the_name_field = () =>
      resultHelper.Model.Name.ShouldEqual(division.Name);

    It should_initialize_the_starting_date_field = () =>
      resultHelper.Model.StartingDate.ShouldEqual(division.StartingDate.ToShortDateString());

    It should_initialize_the_season_name_field = () =>
      resultHelper.Model.SeasonName.ShouldEqual(division.Season.Name);
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_edit_a_division : specification_for_Divisions_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 1;
    static EditDivisionViewModel viewModel;
    static Division division;

    Establish context = () => {
      viewModel = new EditDivisionViewModel();
      viewModel.Id = id;
      viewModel.Name = "NewName";
      viewModel.StartingDate = "11/30/2010";

      division = new Division("temp", DateTime.Now);
      division.Season = new Season("temp");
      division.Season.SetIdTo(1);
      division.SetIdTo(id);
      divisionsRepository.Stub(r => r.Get(id)).Return(division);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_update_the_name = () =>
      division.Name.ShouldEqual(viewModel.Name);

    It should_update_the_starting_date = () =>
      division.StartingDate.ToShortDateString().ShouldEqual(viewModel.StartingDate);

    It should_return_a_notification_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_view_season_view = () =>
      resultHelper.ShouldRedirectTo("seasons", "view");
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_edit_a_division_with_a_model_error : specification_for_Divisions_controller
  {
    static ViewResultHelper<EditDivisionViewModel> resultHelper;
    static int id = 1;
    static EditDivisionViewModel viewModel;

    Establish context = () => {
      viewModel = new EditDivisionViewModel();
      viewModel.Id = id;
    };

    Because of = () => resultHelper = new ViewResultHelper<EditDivisionViewModel>(controller.Edit(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_is_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();

    It should_retain_the_data_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_edit_a_division_with_an_invalid_date : specification_for_Divisions_controller
  {
    static ViewResultHelper<EditDivisionViewModel> resultHelper;
    static int id = 1;
    static EditDivisionViewModel viewModel;

    Establish context = () => {
      viewModel = new EditDivisionViewModel();
      viewModel.Id = id;
      viewModel.Name = "name";
      viewModel.StartingDate = "some bad date";
    };

    Because of = () => resultHelper = new ViewResultHelper<EditDivisionViewModel>(controller.Edit(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_is_related_to_the_date_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("StartingDate").ShouldBeTrue();

    It should_retain_the_data_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);
  }

  [Subject(typeof(DivisionsController))]
  public class when_asked_to_edit_a_division_with_an_invalid_id : specification_for_Divisions_controller
  {
    static ViewResultHelper<EditDivisionViewModel> resultHelper;
    static int id = 1;
    static EditDivisionViewModel viewModel;

    Establish context = () => {
      viewModel = new EditDivisionViewModel();
      viewModel.Id = id;
      viewModel.Name = "name";
      viewModel.StartingDate = "1/1/2001";
    };

    Because of = () => resultHelper = new ViewResultHelper<EditDivisionViewModel>(controller.Edit(viewModel));

    It should_return_the_default_view = () =>
      resultHelper.Result.ViewName.ShouldBeEmpty();

    It should_return_an_error_message = () =>
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();

    It should_retain_the_data_entered_by_the_user = () =>
      resultHelper.Model.ShouldEqual(viewModel);
  }

}