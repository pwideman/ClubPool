using System.Linq;
using System.Web.Mvc;

using MvcContrib;

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
      int pageSize = 10;
      var query = from s in repository.All<Season>()
                  orderby s.IsActive descending, s.Name descending
                  select new SeasonSummaryViewModel { Season = s };
      var viewModel = new IndexViewModel(query, page.GetValueOrDefault(1), pageSize);
      return View(viewModel);
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
      return this.RedirectToAction(c => c.Index(null));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Edit(int id) {
      var season = repository.Get<Season>(id);
      if (null == season) {
        return HttpNotFound();
      }
      var viewModel = new EditSeasonViewModel(season);
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
        return this.RedirectToAction(c => c.Index(null));
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
      return this.RedirectToAction(c => c.Index(null));
    }

    private ActionResult EditRedirectForConcurrency(int id) {
      TempData[GlobalViewDataProperty.PageErrorMessage] =
        "This season was updated by another user while you were viewing this page. Enter your changes again.";
      return this.RedirectToAction(c => c.Edit(id));
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

      return this.RedirectToAction(c => c.Index(page));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult ChangeActive() {
      var viewModel = new ChangeActiveViewModel();
      var activeSeason = repository.All<Season>().SingleOrDefault(s => s.IsActive);
      if (null != activeSeason) {
        viewModel.CurrentActiveSeasonName = activeSeason.Name;
      }
      viewModel.InactiveSeasons = repository.All<Season>().Where(s => !s.IsActive).Select(s => new SeasonSummaryViewModel { Season = s }).ToList();
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
        return this.RedirectToAction(c => c.ChangeActive());
      }
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The active season has been changed";
      return this.RedirectToAction(c => c.Index(null));
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
