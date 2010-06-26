﻿using System;
using System.Linq;
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
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Queries;
using ClubPool.Web.Controllers.Attributes;

namespace ClubPool.Web.Controllers.Seasons
{
  public class SeasonsController : BaseController
  {
    protected ILinqRepository<Season> seasonRepository;

    public SeasonsController(ILinqRepository<Season> seasonRepo) {
      Check.Require(null != seasonRepo, "seasonRepo cannot be null");

      seasonRepository = seasonRepo;
    }

    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Index(int? page) {
      int pageSize = 10;
      var index = Math.Max(page ?? 1, 1) - 1;
      var total = seasonRepository.GetAll().Count();
      var seasons = seasonRepository.GetAll().Select(s => new SeasonDto(s)).Page(index, pageSize).ToList();
      var first = index * pageSize + 1;
      var lastPage = (int)Math.Ceiling((double)total / (double)pageSize);
      var viewModel = new IndexViewModel() {
        Seasons = seasons,
        CurrentPage = index+1,
        Total = total,
        First = first,
        Last = first + seasons.Count - 1,
        TotalPages = lastPage
      };
      return View(viewModel);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Create() {
      var dto = new SeasonDto();
      return View(dto);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Create(SeasonDto dto) {
      try {
        dto.Validate();
      }
      catch (RulesException re) {
        re.AddModelStateErrors(this.ModelState, null);
        return View(dto);
      }

      var season = new Season(dto.Name);
      seasonRepository.SaveOrUpdate(season);
      
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The season was created successfully";
      return this.RedirectToAction(c => c.Index(null));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Edit(int id) {
      var season = seasonRepository.Get(id);
      var dto = new SeasonDto(season);
      return View(dto);
    }

    [HttpPost]
    [Transaction]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(SeasonDto dto) {
      try {
        dto.Validate();
      }
      catch (RulesException re) {
        re.AddModelStateErrors(this.ModelState, null);
        return View(dto);
      }

      var season = seasonRepository.Get(dto.Id);

      if (!season.Name.Equals(dto.Name)) {
        // verify that the new name is not in use
        if (seasonRepository.GetAll().WithName(dto.Name).Any()) {
          ModelState.AddModelErrorFor<SeasonDto>(m => m.Name, "The name is already in use");
          return View(dto);
        }
        season.Name = dto.Name;
      }
      seasonRepository.SaveOrUpdate(season);

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The season was updated successfully";
      return this.RedirectToAction(c => c.Index(null));
    }

    [Authorize(Roles = Roles.Administrators)]
    [HttpPost]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, int page) {
      var seasonToDelete = seasonRepository.Get(id);
      if (null != seasonToDelete) {
        if (seasonToDelete.CanDelete()) {
          seasonRepository.Delete(seasonToDelete);
          TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was deleted successfully.";
        }
        else {
          TempData[GlobalViewDataProperty.PageErrorMessage] = "There is other data in the system that references this season, it cannot be deleted.";
        }
      }
      else {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "Invalid user id";
      }
      return this.RedirectToAction(c => c.Index(page));
    }

  }
}