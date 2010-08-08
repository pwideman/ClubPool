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
using ClubPool.Web.Controllers.Seasons;
using ClubPool.Web.Controllers.Seasons.ViewModels;
using ClubPool.Framework.NHibernate;
using ClubPool.Framework.Extensions;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers.Seasons
{
  public abstract class specification_for_Seasons_controller
  {
    protected static SeasonsController controller;
    protected static ISeasonRepository seasonsRepository;
    protected static IDivisionRepository divisionsRepository;
    protected static ActionResult result;

    Establish context = () => {
      seasonsRepository = MockRepository.GenerateStub<ISeasonRepository>();
      divisionsRepository = MockRepository.GenerateStub<IDivisionRepository>();
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
    static ViewResultHelper<IndexViewModel> resultHelper;

    Establish context = () => {
      var seasons = new List<Season>();
      for (var i = 0; i < pages * pageSize; i++) {
        seasons.Add(new Season("season" + i.ToString()));
      }
      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
    };

    Because of = () => resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index(page));

    It should_set_the_number_of_seasons_to_the_page_size = () =>
      resultHelper.Model.Items.Count().ShouldEqual(pageSize);

    It should_set_the_first_season_index = () =>
      resultHelper.Model.First.ShouldEqual((page - 1) * pageSize + 1);

    It should_set_the_last_season_index = () =>
      resultHelper.Model.Last.ShouldEqual(pageSize * page);

    It should_set_the_current_page_index = () =>
      resultHelper.Model.CurrentPage.ShouldEqual(page);

    It should_set_the_total_number_of_seasons = () =>
      resultHelper.Model.Total.ShouldEqual(pageSize * pages);

    It should_set_the_total_pages = () =>
      resultHelper.Model.TotalPages.ShouldEqual(pages);
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_create_a_season : specification_for_Seasons_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static CreateSeasonViewModel viewModel;
    static string name = "NewSeason";
    static Season savedSeason;

    Establish context = () => {
      viewModel = new CreateSeasonViewModel();
      viewModel.Name = name;

      seasonsRepository.Stub(r => r.SaveOrUpdate(null)).IgnoreArguments().WhenCalled(m => savedSeason = m.Arguments[0] as Season);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Create(viewModel));

    It should_redirect_to_the_default_view = () =>
      resultHelper.ShouldRedirectTo("seasons");

    It should_return_a_notification_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_save_the_new_season = () =>
      savedSeason.ShouldNotBeNull();

    It should_set_the_name_of_the_new_season = () =>
      savedSeason.Name.ShouldEqual(name);
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_create_a_season_with_invalid_data : specification_for_Seasons_controller
  {
    static ViewResultHelper<CreateSeasonViewModel> resultHelper;

    Because of = () => resultHelper = new ViewResultHelper<CreateSeasonViewModel>(controller.Create(new CreateSeasonViewModel()));

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_delete_a_season : specification_for_Seasons_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 1;
    static int page = 2;
    static KeyValuePair<string, object> pageRouteValue;

    Establish context = () => {
      var season = new Season("Test");
      seasonsRepository.Stub(r => r.Get(id)).Return(season);
      seasonsRepository.Expect(r => r.Delete(season));
      pageRouteValue = new KeyValuePair<string, object>("page", page);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Delete(id, page));

    It should_delete_the_season = () =>
      seasonsRepository.VerifyAllExpectations();

    It should_return_a_notification_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_default_view = () =>
      resultHelper.ShouldRedirectTo("seasons");

    It should_redisplay_the_previous_page = () =>
      resultHelper.Result.RouteValues.ShouldContain(pageRouteValue);
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_delete_an_invalid_season : specification_for_Seasons_controller
  {
    static int id = 0;
    static int page = 2;
    static Exception Exception;

    Establish context = () => {
      seasonsRepository.Stub(r => r.Get(id)).Return(null);
    };

    Because of = () => Exception = Catch.Exception(() => controller.Delete(id, page));

    It should_return_an_http_error = () =>
      Exception.ShouldBeOfType<HttpException>();

    // TODO: Debug
    //It should_return_a_status_code_of_404_not_found = () =>
      //Exception.ErrorCode.ShouldEqual((int)HttpStatusCode.NotFound);
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_delete_a_season_that_cannot_be_deleted : specification_for_Seasons_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 0;
    static int page = 2;
    static KeyValuePair<string, object> pageRouteValue;

    Establish context = () => {
      var season = new Season("name");
      season.IsActive = true; // will make CanDelete() return false
      seasonsRepository.Stub(r => r.Get(id)).Return(season);
      pageRouteValue = new KeyValuePair<string, object>("page", page);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Delete(id, page));

    It should_return_an_error_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageErrorMessage).ShouldBeTrue();

    It should_redirect_to_the_default_view = () =>
      resultHelper.ShouldRedirectTo("seasons");

    It should_redisplay_the_previous_page = () =>
      resultHelper.Result.RouteValues.ShouldContain(pageRouteValue);
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_for_the_change_active_season_view : specification_for_Seasons_controller
  {
    static ViewResultHelper<ChangeActiveViewModel> resultHelper;
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

    Because of = () => resultHelper = new ViewResultHelper<ChangeActiveViewModel>(controller.ChangeActive());

    It should_return_the_correct_current_active_season = () =>
      resultHelper.Model.CurrentActiveSeasonName.ShouldEqual(name);

    It should_return_the_inactive_seasons = () =>
      seasons.Where(s => !s.IsActive).Each(s => resultHelper.Model.InactiveSeasons.Select(inactiveSeason => inactiveSeason.Id).ShouldContain(s.Id));
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_change_the_active_season : specification_for_Seasons_controller
  {
    static RedirectToRouteResultHelper resultHelper;
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

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.ChangeActive(newActiveSeasonId));

    It should_set_the_previous_active_season_to_inactive = () =>
      activeSeason.IsActive.ShouldBeFalse();

    It should_set_the_new_active_season_to_active = () =>
      newActiveSeason.IsActive.ShouldBeTrue();

    It should_save_the_new_active_season = () =>
      seasonsRepository.VerifyAllExpectations();

    It should_return_a_notification_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_default_view = () =>
      resultHelper.ShouldRedirectTo("seasons");
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_change_the_active_season_to_an_invalid_season : specification_for_Seasons_controller
  {
    static int badId = -1;
    static List<Season> seasons;
    static Season activeSeason;
    static Exception Exception;

    Establish context = () => {
      activeSeason = new Season("name");
      activeSeason.IsActive = true;
      seasons = new List<Season>();
      seasons.Add(activeSeason);

      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
    };

    Because of = () => Exception = Catch.Exception(() => controller.ChangeActive(badId));

    It should_not_change_the_active_season = () =>
      activeSeason.IsActive.ShouldBeTrue();

    It should_not_save_anything = () =>
      seasonsRepository.AssertWasNotCalled(r => r.SaveOrUpdate(null), o => o.IgnoreArguments());

    It should_return_an_http_error = () =>
      Exception.ShouldBeOfType<HttpException>();

    // TODO: Debug
    //It should_return_a_status_code_of_404_not_found = () =>
    //Exception.ErrorCode.ShouldEqual((int)HttpStatusCode.NotFound);
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_for_the_edit_view : specification_for_Seasons_controller
  {
    static ViewResultHelper<EditSeasonViewModel> resultHelper;
    static int id = 1;
    static string name = "name";

    Establish context = () => {
      var season = new Season(name);

      seasonsRepository.Stub(r => r.Get(id)).Return(season);
    };

    Because of = () => resultHelper = new ViewResultHelper<EditSeasonViewModel>(controller.Edit(id));

    It should_initialize_the_name_field = () =>
      resultHelper.Model.Name.ShouldEqual(name);
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_edit_a_season : specification_for_Seasons_controller
  {
    static RedirectToRouteResultHelper resultHelper;
    static int id = 1;
    static string name = "name";
    static Season season;
    static EditSeasonViewModel viewModel;

    Establish context = () => {
      season = new Season("temp");
      season.SetIdTo(id);

      viewModel = new EditSeasonViewModel(season);
      viewModel.Name = name;

      seasonsRepository.Stub(r => r.Get(id)).Return(season);
      seasonsRepository.Stub(r => r.GetAll()).Return(new List<Season>() { season }.AsQueryable());
      seasonsRepository.Expect(r => r.SaveOrUpdate(season)).IgnoreArguments().Return(season);
    };

    Because of = () => resultHelper = new RedirectToRouteResultHelper(controller.Edit(viewModel));

    It should_save_the_season = () =>
      seasonsRepository.VerifyAllExpectations();

    It should_update_the_season_name = () =>
      season.Name.ShouldEqual(name);

    It should_return_a_notification_message = () =>
      controller.TempData.Keys.Contains(GlobalViewDataProperty.PageNotificationMessage).ShouldBeTrue();

    It should_redirect_to_the_default_view = () =>
      resultHelper.ShouldRedirectTo("seasons");
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_edit_a_season_with_invalid_data : specification_for_Seasons_controller
  {
    static ViewResultHelper<EditSeasonViewModel> resultHelper;
    static EditSeasonViewModel viewModel;

    Establish context = () => {
      viewModel = new EditSeasonViewModel();
    };

    Because of = () => resultHelper = new ViewResultHelper<EditSeasonViewModel>(controller.Edit(viewModel));

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();
  }

  [Subject(typeof(SeasonsController))]
  public class when_asked_to_edit_a_season_with_a_duplicate_name : specification_for_Seasons_controller
  {
    static ViewResultHelper<EditSeasonViewModel> resultHelper;
    static EditSeasonViewModel viewModel;
    static int id = 1;

    Establish context = () => {
      var seasons = new List<Season>();
      for (int i = 1; i < 4; i++) {
        var season = new Season("season" + i.ToString());
        season.SetIdTo(i);
        seasons.Add(season);
      }
      viewModel = new EditSeasonViewModel(seasons[0]);
      viewModel.Name = seasons[1].Name;

      seasonsRepository.Stub(r => r.Get(id)).Return(seasons[0]);
      seasonsRepository.Stub(r => r.GetAll()).Return(seasons.AsQueryable());
    };

    Because of = () => resultHelper = new ViewResultHelper<EditSeasonViewModel>(controller.Edit(viewModel));

    It should_indicate_an_error = () =>
      resultHelper.Result.ViewData.ModelState.IsValid.ShouldBeFalse();

    It should_indicate_the_error_was_related_to_the_name_field = () =>
      resultHelper.Result.ViewData.ModelState.ContainsKey("Name").ShouldBeTrue();
  }

}
