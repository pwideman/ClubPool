using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.Web.Controllers.Divisions.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.Web.Controllers.Attributes;
using ClubPool.ApplicationServices.DomainManagement.Contracts;

namespace ClubPool.Web.Controllers.Divisions
{

  public class DivisionsController : BaseController
  {
    protected ISeasonRepository seasonRepository;
    protected IDivisionRepository divisionRepository;
    protected IDivisionManagementService divisionManagementService;

    public DivisionsController(IDivisionRepository divisionRepository, ISeasonRepository seasonRepository,
      IDivisionManagementService divisionManagementService) {
      Check.Require(null != seasonRepository, "seasonRepository cannot be null");
      Check.Require(null != divisionRepository, "divisionRepository cannot be null");
      Check.Require(null != divisionManagementService, "divisionManagementService cannot be null");

      this.seasonRepository = seasonRepository;
      this.divisionRepository = divisionRepository;
      this.divisionManagementService = divisionManagementService;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Create(int seasonId) {
      var season = seasonRepository.Get(seasonId);
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
    [Transaction]
    public ActionResult Create(CreateDivisionViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }
      DateTime startingDate;
      if (!DateTime.TryParse(viewModel.StartingDate, out startingDate)) {
        ModelState.AddModelErrorFor<CreateDivisionViewModel>(m => m.StartingDate, "Enter a valid date");
        return View(viewModel);
      }

      Season season = seasonRepository.Get(viewModel.SeasonId);
      if (null == season) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "Invalid/missing season, cannot create division";
        return View(viewModel);
      }

      if (divisionManagementService.DivisionNameIsInUse(season, viewModel.Name)) {
        ModelState.AddModelErrorFor<CreateDivisionViewModel>(m => m.Name, "This name is already in use");
        return View(viewModel);
      }

      // TODO: division constructor needs to require season
      Division division = new Division(viewModel.Name, startingDate);
      divisionRepository.SaveOrUpdate(division);
      season.AddDivision(division);

      // I hate doing this here because theoretically /divisions/create could be called
      // from anywhere, but I don't know what else to do now. Same comment applies for all
      // redirects in this controller
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(season.Id));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id) {
      var division = divisionRepository.Get(id);
      if (null == division) {
        return HttpNotFound();
      }
      var seasonId = division.Season.Id;
      if (!division.CanDelete()) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The division cannot be deleted";
      }
      else {
        divisionRepository.Delete(division);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The division was deleted";
      }
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(seasonId));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Edit(int id) {
      var division = divisionRepository.Get(id);
      if (null == division) {
        return HttpNotFound();
      }
      var viewModel = new EditDivisionViewModel();
      viewModel.Id = id;
      viewModel.Name = division.Name;
      viewModel.SeasonName = division.Season.Name;
      viewModel.StartingDate = division.StartingDate.ToShortDateString();
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditDivisionViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }
      DateTime startingDate;
      if (!DateTime.TryParse(viewModel.StartingDate, out startingDate)) {
        ModelState.AddModelErrorFor<EditDivisionViewModel>(m => m.StartingDate, "Enter a valid date");
        return View(viewModel);
      }

      var division = divisionRepository.Get(viewModel.Id);
      if (!division.Name.Equals(viewModel.Name)) {
        // verify that the new name is not already in use
        if (divisionManagementService.DivisionNameIsInUse(division.Season, viewModel.Name)) {
          ModelState.AddModelErrorFor<EditDivisionViewModel>(m => m.Name, "Name is in use");
          return View(viewModel);
        }
        division.Name = viewModel.Name;
      }
      division.StartingDate = startingDate;

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The division was updated";
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(division.Season.Id));
    }
  }

}
