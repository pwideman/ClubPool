﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.Web.Controllers.Seasons.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Web.Infrastructure;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.Web.Controllers.Attributes;

namespace ClubPool.Web.Controllers.Seasons
{
  public class SeasonsController : BaseController
  {
    protected ISeasonRepository seasonRepository;
    protected IDivisionRepository divisionRepository;

    public SeasonsController(ISeasonRepository seasonRepo, IDivisionRepository divisionRepo) {
      Check.Require(null != seasonRepo, "seasonRepo cannot be null");
      Check.Require(null != divisionRepo, "divisionRepo cannot be null");

      seasonRepository = seasonRepo;
      divisionRepository = divisionRepo;
    }

    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Index(int? page) {
      int pageSize = 10;
      var query = from s in seasonRepository.GetAll()
                  orderby s.IsActive descending, s.Name descending
                  select new SeasonSummaryViewModel(s);
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
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateSeasonViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }

      var season = new Season(viewModel.Name, GameType.EightBall);
      seasonRepository.SaveOrUpdate(season);
      
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The season was created successfully";
      return this.RedirectToAction(c => c.Index(null));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Edit(int id) {
      var season = seasonRepository.Get(id);
      if (null == season) {
        return HttpNotFound();
      }
      var viewModel = new EditSeasonViewModel(season);
      return View(viewModel);
    }

    [HttpPost]
    [Transaction]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditSeasonViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }

      var season = seasonRepository.Get(viewModel.Id);

      if (null == season) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The season you were editing was deleted by another user";
        return this.RedirectToAction(c => c.Index(null));
      }

      if (viewModel.Version != season.Version) {
        TempData[GlobalViewDataProperty.PageErrorMessage] =
          "This season was updated by another user while you were viewing this page. Enter your changes again.";
        return this.RedirectToAction(c => c.Edit(viewModel.Id));
      }

      if (!season.Name.Equals(viewModel.Name)) {
        // verify that the new name is not in use
        if (seasonRepository.GetAll().WithName(viewModel.Name).Any()) {
          ModelState.AddModelErrorFor<EditSeasonViewModel>(m => m.Name, "The name is already in use");
          return View(viewModel);
        }
        season.Name = viewModel.Name;
      }

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The season was updated successfully";
      return this.RedirectToAction(c => c.Index(null));
    }

    [Authorize(Roles = Roles.Administrators)]
    [HttpPost]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, int? page) {
      var seasonToDelete = seasonRepository.Get(id);

      if (null == seasonToDelete) {
        return HttpNotFound();
      }

      if (seasonToDelete.CanDelete()) {
        seasonRepository.Delete(seasonToDelete);
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
    [Transaction]
    public ActionResult ChangeActive() {
      var viewModel = new ChangeActiveViewModel();
      var activeSeason = seasonRepository.GetAll().WhereActive().SingleOrDefault();
      if (null != activeSeason) {
        viewModel.CurrentActiveSeasonName = activeSeason.Name;
      }
      viewModel.InactiveSeasons = seasonRepository.GetAll().WhereInactive().Select(s => new SeasonSummaryViewModel(s)).ToList();
      return View(viewModel);
    }

    [HttpPost]
    [Transaction]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult ChangeActive(int id) {
      var newActiveSeason = seasonRepository.Get(id);
      
      if (null == newActiveSeason) {
        return HttpNotFound();
      }

      var activeSeasons = seasonRepository.GetAll().WhereActive();
      foreach (var season in activeSeasons) {
        season.IsActive = false;
      }
      newActiveSeason.IsActive = true;
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The active season has been changed";
      return this.RedirectToAction(c => c.Index(null));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult View(int id) {
      var season = seasonRepository.Get(id);
      if (null == season) {
        return HttpNotFound();
      }
      var viewModel = new SeasonViewModel(season);
      return View(viewModel);
    }

  }
}