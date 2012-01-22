using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using ClubPool.Web.Models;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Schedule
{
  public class ScheduleController : BaseController
  {
    protected IRepository repository;
    protected IAuthenticationService authService;

    public ScheduleController(IRepository repo, IAuthenticationService authService) {
      Arg.NotNull(repo, "repo");
      Arg.NotNull(authService, "authService");

      repository = repo;
      this.authService = authService;
    }

    [Authorize]
    [HttpGet]
    public ActionResult Index() {
      var user = repository.Get<User>(authService.GetCurrentPrincipal().UserId);
      var season = repository.All<Season>().SingleOrDefault(s => s.IsActive);
      if (null == season) {
        return ErrorView("There is no current season");
      }
      else {
        var viewModel = CreateSeasonScheduleViewModel(season, user);
        return View(viewModel);
      }
    }

    private SeasonScheduleViewModel CreateSeasonScheduleViewModel(Season season, User user) {
      var model = new SeasonScheduleViewModel();
      model.Name = season.Name;
      if (season.Divisions.Count() > 0) {
        model.HasDivisions = true;
        var divisions = new List<DivisionScheduleViewModel>();
        foreach (var division in season.Divisions) {
          divisions.Add(CreateDivisionScheduleViewModel(division, user));
        }
        model.Divisions = divisions;
      }
      else {
        model.HasDivisions = false;
      }
      return model;
    }

    private DivisionScheduleViewModel CreateDivisionScheduleViewModel(Division division, User user) {
      var model = new DivisionScheduleViewModel();
      model.Id = division.Id;
      model.Name = division.Name;
      if (division.Meets.Count() > 0) {
        model.HasSchedule = true;
        var team = division.Teams.Where(t => t.Players.Contains(user)).SingleOrDefault();
        model.Schedule = new ScheduleViewModel(division.Meets, division.StartingDate, team);
      }
      else {
        model.HasSchedule = false;
      }
      return model;
    }
  }
}
