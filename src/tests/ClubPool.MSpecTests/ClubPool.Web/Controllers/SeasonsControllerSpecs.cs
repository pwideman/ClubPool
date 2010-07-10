using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Principal;

using Rhino.Mocks;
using Machine.Specifications;
using SharpArch.Testing;

using ClubPool.Core;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Seasons;
using ClubPool.Web.Controllers.Seasons.ViewModels;
using ClubPool.Framework.NHibernate;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Seasons
{
  public abstract class specification_for_Seasons_controller
  {
    protected static SeasonsController controller;
    protected static ILinqRepository<Season> seasonsRepository;
    protected static ILinqRepository<Division> divisionsRepository;
    protected static ActionResult result;

    Establish context = () => {
      seasonsRepository = MockRepository.GenerateStub<ILinqRepository<Season>>();
      divisionsRepository = MockRepository.GenerateStub<ILinqRepository<Division>>();
      controller = new SeasonsController(seasonsRepository, divisionsRepository);

      ControllerHelper.CreateMockControllerContext(controller);
      ServiceLocatorHelper.AddValidator();
    };
  }

  // We don't need to test the paging here, that is tested in PagedListViewModelBaseSpecs
  [Subject(typeof(SeasonsController))]
  public class when_asked_for_the_default_view : specification_for_Seasons_controller
  {
    static int page = 1;
    static int pages = 3;
    static int pageSize = 10;

    Establish context = () => {
      var seasons = new List<Season>();
      for (var i = 0; i < pages * pageSize; i++) {
        seasons.Add(new Season("season" + i.ToString()));
      }
      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
    };

    Because of = () => result = controller.Index(page);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_for_the_create_view : specification_for_Seasons_controller
  {

    Because of = () => result = controller.Create();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_create_a_season : specification_for_Seasons_controller
  {
    static SeasonDto viewModel;
    static string name = "NewSeason";
    static Season savedSeason;

    Establish context = () => {
      viewModel = new SeasonDto();
      viewModel.Name = name;

      seasonsRepository.Stub(r => r.SaveOrUpdate(null)).IgnoreArguments().WhenCalled(m => savedSeason = m.Arguments[0] as Season);
    };

    Because of = () => result = controller.Create(viewModel);

    It should_redirect_to_the_default_view = () => {
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("seasons");
    };

    It should_set_the_page_notification_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_save_the_season = () => {
      savedSeason.ShouldNotBeNull();
      savedSeason.Name.ShouldEqual(name);
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_create_a_season_with_invalid_data : specification_for_Seasons_controller
  {

    Because of = () => result = controller.Create(new SeasonDto());

    It should_return_the_default_view = () => {
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
    };

    It should_add_the_model_error = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.Count.ShouldBeGreaterThan(0);
      modelState.Keys.Contains("Name").ShouldBeTrue();
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_delete_a_season : specification_for_Seasons_controller
  {
    static int id = 1;
    static int page = 2;

    Establish context = () => {
      var season = new Season("Test");
      seasonsRepository.Stub(r => r.Get(id)).Return(season);
      seasonsRepository.Expect(r => r.Delete(season));
    };

    Because of = () => result = controller.Delete(id, page);

    It should_delete_the_season = () =>
      seasonsRepository.VerifyAllExpectations();

    It should_set_the_page_notification_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_index_view = () => {
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("seasons");
      var pageRouteValue = new KeyValuePair<string, object>("page", page);
      result.IsARedirectToARouteAnd().RouteValues.ShouldContain(pageRouteValue);
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_delete_an_invalid_season : specification_for_Seasons_controller
  {
    static int id = 0;
    static int page = 2;

    Establish context = () => {
      seasonsRepository.Stub(r => r.Get(id)).Return(null);
    };

    Because of = () => result = controller.Delete(id, page);

    It should_set_the_page_error_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();

    It should_redirect_to_the_index_view = () => {
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("seasons");
      var pageRouteValue = new KeyValuePair<string, object>("page", page);
      result.IsARedirectToARouteAnd().RouteValues.ShouldContain(pageRouteValue);
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_delete_a_season_that_cannot_be_deleted : specification_for_Seasons_controller
  {
    static int id = 0;
    static int page = 2;

    Establish context = () => {
      var season = new Season("name");
      season.IsActive = true; // will make CanDelete() return false
      seasonsRepository.Stub(r => r.Get(id)).Return(season);
    };

    Because of = () => result = controller.Delete(id, page);

    It should_set_the_page_error_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();

    It should_redirect_to_the_index_view = () => {
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("seasons");
      var pageRouteValue = new KeyValuePair<string, object>("page", page);
      result.IsARedirectToARouteAnd().RouteValues.ShouldContain(pageRouteValue);
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_for_the_change_active_season_view : specification_for_Seasons_controller
  {
    static string name = "active";
    static List<Season> seasons;
    static int inactiveCount = 5;

    Establish context = () => {
      seasons = new List<Season>();
  
      var season = new Season(name);
      season.IsActive = true;
      seasons.Add(season);

      for (int i = 1; i <= inactiveCount; i++) {
        season = new Season("other" + i.ToString());
        seasons.Add(season);
      }
      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
    };

    Because of = () => result = controller.ChangeActive();

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as ChangeActiveViewModel;
      viewModel.ShouldNotBeNull();
      viewModel.CurrentActiveSeasonName.ShouldEqual(name);
      viewModel.InactiveSeasons.Count().ShouldEqual(inactiveCount);
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_change_the_active_season : specification_for_Seasons_controller
  {
    static string name = "active";
    static List<Season> seasons;
    static int inactiveCount = 5;
    static int newActiveSeasonId = 10;
    static Season activeSeason;
    static Season newActiveSeason;

    Establish context = () => {
      seasons = new List<Season>();

      activeSeason = new Season(name);
      activeSeason.IsActive = true;
      seasons.Add(activeSeason);

      for (int i = 1; i <= inactiveCount; i++) {
        var season = new Season("other" + i.ToString());
        seasons.Add(season);
      }

      newActiveSeason = new Season("newactive");
      newActiveSeason.SetIdTo(newActiveSeasonId);
      seasons.Add(newActiveSeason);

      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
      seasonsRepository.Expect(r => r.SaveOrUpdate(newActiveSeason)).Return(newActiveSeason);
      seasonsRepository.Stub(r => r.Get(newActiveSeasonId)).Return(newActiveSeason);
    };

    Because of = () => result = controller.ChangeActive(newActiveSeasonId);

    It should_set_the_previous_active_season_to_inactive = () =>
      activeSeason.IsActive.ShouldBeFalse();

    It should_set_the_new_active_season_to_active = () =>
      newActiveSeason.IsActive.ShouldBeTrue();

    It should_save_the_new_active_season = () =>
      seasonsRepository.VerifyAllExpectations();

    It should_set_the_page_notification_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_index_view = () => {
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("seasons");
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_change_the_active_season_to_an_invalid_season : specification_for_Seasons_controller
  {
    static int badId = -1;
    static List<Season> seasons;
    static Season activeSeason;

    Establish context = () => {
      activeSeason = new Season("name");
      activeSeason.IsActive = true;
      seasons = new List<Season>();
      seasons.Add(activeSeason);

      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
    };

    Because of = () => result = controller.ChangeActive(badId);

    It should_set_the_page_error_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();

    It should_redirect_to_the_index_view = () => {
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("seasons");
    };

    It should_not_change_the_active_season = () =>
      activeSeason.IsActive.ShouldBeTrue();

    It should_not_save_anything = () =>
      seasonsRepository.AssertWasNotCalled(r => r.SaveOrUpdate(null), o => o.IgnoreArguments());
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_for_the_edit_view : specification_for_Seasons_controller
  {
    static int id = 1;
    static string name = "name";

    Establish context = () => {
      var season = new Season(name);

      seasonsRepository.Stub(r => r.Get(id)).Return(season);
    };

    Because of = () => result = controller.Edit(id);

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();

    It should_set_the_view_model_properties = () => {
      var viewModel = result.IsAViewAnd().ViewData.Model as SeasonDto;
      viewModel.ShouldNotBeNull();
      viewModel.Name.ShouldEqual(name);
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_edit_a_season : specification_for_Seasons_controller
  {
    static int id = 1;
    static string name = "name";
    static Season season;
    static SeasonDto viewModel;

    Establish context = () => {
      season = new Season("temp");
      season.SetIdTo(id);

      viewModel = new SeasonDto(season);
      viewModel.Name = name;

      seasonsRepository.Stub(r => r.Get(id)).Return(season);
      seasonsRepository.Stub(r => r.GetAll()).Return(new List<Season>() { season }.AsQueryable());
      seasonsRepository.Expect(r => r.SaveOrUpdate(season)).IgnoreArguments().Return(season);
    };

    Because of = () => result = controller.Edit(viewModel);

    It should_save_the_season = () =>
      seasonsRepository.VerifyAllExpectations();

    It should_update_the_season_properties = () => {
      season.Name.ShouldEqual(name);
    };

    It should_set_the_page_notification_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_index = () => {
      result.IsARedirectToARouteAnd().ActionName().ToLower().ShouldEqual("index");
      result.IsARedirectToARouteAnd().ControllerName().ToLower().ShouldEqual("seasons");
    };
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_edit_a_season_with_invalid_data : specification_for_Seasons_controller
  {
    static SeasonDto viewModel;

    Establish context = () => {
      viewModel = new SeasonDto();
    };

    Because of = () => result = controller.Edit(viewModel);

    It should_set_the_model_state_errors = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.Count.ShouldBeGreaterThan(0);
      modelState.Keys.Contains("Name").ShouldBeTrue();
    };

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_edit_a_season_with_a_duplicate_name : specification_for_Seasons_controller
  {
    static SeasonDto viewModel;
    static int id = 0;

    Establish context = () => {
      var seasons = new List<Season>();
      for (int i = 0; i < 3; i++) {
        var season = new Season("season" + i.ToString());
        season.SetIdTo(i);
        seasons.Add(season);
      }
      viewModel = new SeasonDto(seasons[id]);
      viewModel.Name = seasons[id + 1].Name;

      seasonsRepository.Stub(r => r.Get(id)).Return(seasons[id]);
      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
    };

    Because of = () => result = controller.Edit(viewModel);

    It should_set_the_model_state_errors = () => {
      var modelState = result.IsAViewAnd().ViewData.ModelState;
      modelState.Count.ShouldBeGreaterThan(0);
      modelState.Keys.Contains("Name").ShouldBeTrue();
    };

    It should_return_the_default_view = () =>
      result.IsAViewAnd().ViewName.ShouldBeEmpty();
  }

}
