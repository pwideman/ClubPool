using System;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Divisions
{

  public class DivisionsController : BaseController
  {
    protected IRepository repository;

    public DivisionsController(IRepository repository) {
      Arg.NotNull(repository, "repository");

      this.repository = repository;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Create(int seasonId) {
      var season = repository.Get<Season>(seasonId);
      if (null == season) {
        return HttpNotFound();
      }
      var viewModel = new CreateDivisionViewModel() {
        SeasonId = season.Id,
        SeasonName = season.Name,
      };
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateDivisionViewModel viewModel) {
      if (!ModelState.IsValid) {
        return View(viewModel);
      }
      DateTime startingDate;
      if (!DateTime.TryParse(viewModel.StartingDate, out startingDate)) {
        ModelState.AddModelErrorFor<CreateDivisionViewModel>(m => m.StartingDate, "Enter a valid date");
        return View(viewModel);
      }

      Season season = repository.Get<Season>(viewModel.SeasonId);
      if (null == season) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "Invalid/missing season, cannot create division";
        return View(viewModel);
      }

      if (season.DivisionNameIsInUse(viewModel.Name)) {
        ModelState.AddModelErrorFor<CreateDivisionViewModel>(m => m.Name, "This name is already in use");
        return View(viewModel);
      }

      Division division = new Division(viewModel.Name, startingDate, season);
      repository.SaveOrUpdate(division);
      season.AddDivision(division);
      repository.SaveChanges();

      // I hate doing this here because theoretically /divisions/create could be called
      // from anywhere, but I don't know what else to do now. Same comment applies for all
      // redirects in this controller
      return RedirectToAction("Details", "Seasons", new { id = season.Id });
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id) {
      var division = repository.Get<Division>(id);
      if (null == division) {
        return HttpNotFound();
      }
      var seasonId = division.Season.Id;
      if (!division.CanDelete()) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The division cannot be deleted";
      }
      else {
        DeleteDivision(division);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The division was deleted";
      }
      return RedirectToAction("Details", "Seasons", new { id = seasonId });
    }

    private void DeleteDivision(Division division) {
      DeleteDivisionMeets(division);
      DeleteDivisionTeams(division);
      repository.Delete(division);
    }

    private void DeleteDivisionMeets(Division division) {
      var meetsToDelete = repository.All<Meet>().Where(m => m.Division.Id == division.Id).ToList();
      foreach (var meet in meetsToDelete) {
        repository.Delete(meet);
      }
    }

    private void DeleteDivisionTeams(Division division) {
      var teamsToDelete = repository.All<Team>().Where(t => t.Division.Id == division.Id);
      foreach (var team in teamsToDelete) {
        repository.Delete(team);
      }
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Edit(int id) {
      var division = repository.Get<Division>(id);
      if (null == division) {
        return HttpNotFound();
      }
      var viewModel = CreateEditDivisionViewModel(division);
      return View(viewModel);
    }

    private EditDivisionViewModel CreateEditDivisionViewModel(Division division) {
      var model = new EditDivisionViewModel() {
        Id = division.Id,
        Version = division.EncodedVersion,
        SeasonName = division.Season.Name,
        Name = division.Name,
        StartingDate = division.StartingDate.ToShortDateString()
      };
      return model;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditDivisionViewModel viewModel) {
      if (!ModelState.IsValid) {
        return View(viewModel);
      }
      var division = repository.Get<Division>(viewModel.Id);

      // loading the season name here serves two purposes:
      // (1) reload the season name in case we have to redisplay the view due to error
      // (2) lazily load the division.Season property, because EF is too stupid
      // to update the division without Season being set, it thinks you're trying
      // to set the season to null
      viewModel.SeasonName = division.Season.Name;

      DateTime startingDate;
      if (!DateTime.TryParse(viewModel.StartingDate, out startingDate)) {
        ModelState.AddModelErrorFor<EditDivisionViewModel>(m => m.StartingDate, "Enter a valid date");
        return View(viewModel);
      }

      if (null == division) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The division you were editing was deleted by another user";
        return RedirectToAction("Index", "Seasons");
      }

      if (viewModel.Version != division.EncodedVersion) {
        return EditRedirectForConcurrency(viewModel.Id);
      }
      
      if (!division.Name.Equals(viewModel.Name)) {
        // verify that the new name is not already in use
        if (division.Season.DivisionNameIsInUse(viewModel.Name)) {
          ModelState.AddModelErrorFor<EditDivisionViewModel>(m => m.Name, "Name is in use");
          viewModel.Name = division.Name;
          return View(viewModel);
        }
        division.Name = viewModel.Name;
      }
      division.StartingDate = startingDate;

      try {
        repository.SaveChanges();
      }
      catch (UpdateConcurrencyException) {
        return EditRedirectForConcurrency(viewModel.Id);
      }
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The division was updated";
      return RedirectToAction("Details", "Seasons", new { id = division.Season.Id });
    }

    private ActionResult EditRedirectForConcurrency(int id) {
      TempData[GlobalViewDataProperty.PageErrorMessage] =
        "This division was updated by another user while you were viewing this page. Enter your changes again.";
      return RedirectToAction("Edit", new { id = id });
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult CreateSchedule(int id, int? byes) {
      if (null == byes) {
        byes = 0;
      }
      var division = repository.Get<Division>(id);
      try {
        division.CreateSchedule(repository, (int)byes);
      }
      catch (CreateScheduleException e) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = e.Message;
      }
      repository.SaveChanges();
      return RedirectToAction("Details", "Seasons", new { id = division.Season.Id });
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult ClearSchedule(int id) {
      var division = repository.Get<Division>(id);
      if (!division.HasCompletedMatches()) {
        DeleteDivisionMeets(division);
      }
      return RedirectToAction("Details", "Seasons", new { id = division.Season.Id });
    }
  }

}
