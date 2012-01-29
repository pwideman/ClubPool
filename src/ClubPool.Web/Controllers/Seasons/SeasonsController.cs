using System;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Seasons.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Seasons
{
  public class SeasonsController : BaseController
  {
    protected IRepository repository;

    public SeasonsController(IRepository repo) {
      Arg.NotNull(repo, "repo");

      repository = repo;
    }

    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Index(int? page) {
      var query = repository.All<Season>().OrderByDescending(s => s.IsActive).ThenByDescending(s => s.Name);
      var viewModel = CreateIndexViewModel(query, page.GetValueOrDefault(1));
      return View(viewModel);
    }

    private IndexViewModel CreateIndexViewModel(IQueryable<Season> seasons, int page) {
      var model = new IndexViewModel();
      InitializePagedListViewModel(model, seasons, page, 10, (s) => new SeasonSummaryViewModel() {
        Id = s.Id,
        Name = s.Name,
        IsActive = s.IsActive,
        CanDelete = s.CanDelete()
      });
      return model;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Create() {
      var viewModel = new CreateSeasonViewModel();
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateSeasonViewModel viewModel) {
      if (!ModelState.IsValid) {
        return View(viewModel);
      }

      var season = new Season(viewModel.Name, GameType.EightBall);
      repository.SaveOrUpdate(season);
      repository.SaveChanges();
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The season was created successfully";
      return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Edit(int id) {
      var season = repository.Get<Season>(id);
      if (null == season) {
        return HttpNotFound();
      }
      var viewModel = new EditSeasonViewModel() {
        Id = season.Id,
        Name = season.Name,
        Version = season.EncodedVersion
      };
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditSeasonViewModel viewModel) {
      if (!ModelState.IsValid) {
        return View(viewModel);
      }

      var season = repository.Get<Season>(viewModel.Id);

      if (null == season) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The season you were editing was deleted by another user";
        return RedirectToAction("Index");
      }

      if (viewModel.Version != season.EncodedVersion) {
        return EditRedirectForConcurrency(viewModel.Id);
      }

      if (!season.Name.Equals(viewModel.Name)) {
        // verify that the new name is not in use
        if (repository.All<Season>().Any(s => s.Name.Equals(viewModel.Name))) {
          ModelState.AddModelErrorFor<EditSeasonViewModel>(m => m.Name, "The name is already in use");
          return View(viewModel);
        }
        season.Name = viewModel.Name;
      }

      try {
        repository.SaveChanges();
      }
      catch (UpdateConcurrencyException) {
        return EditRedirectForConcurrency(viewModel.Id);
      }

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The season was updated successfully";
      return RedirectToAction("Index");
    }

    private ActionResult EditRedirectForConcurrency(int id) {
      TempData[GlobalViewDataProperty.PageErrorMessage] =
        "This season was updated by another user while you were viewing this page. Enter your changes again.";
      return RedirectToAction("Edit", new { id = id });
    }

    [Authorize(Roles = Roles.Administrators)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, int? page) {
      var seasonToDelete = repository.Get<Season>(id);

      if (null == seasonToDelete) {
        return HttpNotFound();
      }

      if (seasonToDelete.CanDelete()) {
        repository.Delete(seasonToDelete);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The season was deleted successfully.";
      }
      else {
        TempData[GlobalViewDataProperty.PageErrorMessage] = 
          "There are completed matches in this season, it cannot be deleted.";
      }

      return RedirectToAction("Index", new { page = page });
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult ChangeActive() {
      var viewModel = new ChangeActiveViewModel();
      var activeSeason = repository.All<Season>().SingleOrDefault(s => s.IsActive);
      if (null != activeSeason) {
        viewModel.CurrentActiveSeasonName = activeSeason.Name;
      }
      viewModel.InactiveSeasons = repository.All<Season>().Where(s => !s.IsActive).ToList().Select(s => new SeasonSummaryViewModel() {
        Id = s.Id,
        Name = s.Name
      });
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult ChangeActive(int id) {
      var newActiveSeason = repository.Get<Season>(id);
      
      if (null == newActiveSeason) {
        return HttpNotFound();
      }

      var activeSeasons = repository.All<Season>().Where(s => s.IsActive);
      foreach (var season in activeSeasons) {
        season.IsActive = false;
      }
      newActiveSeason.IsActive = true;
      try {
        repository.SaveChanges();
      }
      catch (UpdateConcurrencyException) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "One or more of the seasons were updated by another user, make your changes again.";
        return RedirectToAction("ChangeActive");
      }
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The active season has been changed";
      return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult View(int id) {
      var season = repository.Get<Season>(id);
      if (null == season) {
        return HttpNotFound();
      }
      var viewModel = new SeasonViewModel(season);
      return View(viewModel);
    }

  }
}
