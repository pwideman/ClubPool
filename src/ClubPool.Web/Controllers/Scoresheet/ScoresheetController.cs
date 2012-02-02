using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Scoresheet
{
  public class ScoresheetController : Controller, IRouteRegistrar
  {
    private IRepository repository;

    public ScoresheetController(IRepository repository) {
      Arg.NotNull(repository, "repository");
      this.repository = repository;
    }

    [Authorize]
    public ActionResult Scoresheet(int id) {
      var meet = repository.Get<Meet>(id);
      var viewModel = CreateScoresheetViewModel(meet);
      return View(viewModel);
    }

    private ScoresheetViewModel CreateScoresheetViewModel(Meet meet) {
      var model = new ScoresheetViewModel();
      model.Id = meet.Id;
      model.ScheduledWeek = meet.Week + 1;
      model.ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7);
      var teams = meet.Teams.ToArray();
      var team1 = teams[0];
      var team2 = teams[1];
      model.Team1Name = team1.Name;
      model.Team2Name = team2.Name;
      var matches = new List<ScoresheetMatchViewModel>();
      foreach (var match in meet.Matches) {
        matches.Add(CreateScoresheetMatchViewModel(match));
      }
      model.Matches = matches;
      return model;
    }

    private ScoresheetMatchViewModel CreateScoresheetMatchViewModel(Match match) {
      var model = new ScoresheetMatchViewModel();
      model.Id = match.Id;
      var gameType = match.Meet.Division.Season.GameType;
      var players = match.Players.ToArray();
      model.Player1 = CreatePlayerViewModel(players[0].Player, gameType);
      model.Player2 = CreatePlayerViewModel(players[1].Player, gameType);

      model.Player1.GamesToWin = CalculateGamesToWin(model.Player1.SkillLevel, model.Player2.SkillLevel);
      model.Player2.GamesToWin = CalculateGamesToWin(model.Player2.SkillLevel, model.Player1.SkillLevel);
      return model;
    }

    private int CalculateGamesToWin(int skillLevel, int opponentSkillLevel) {
      int gtw = 0;
      int maxDifference = 1; // number of games to reduce skill level by
      // compute GTW
      if (0 == skillLevel || 0 == opponentSkillLevel) {
        gtw = 4;
      }
      else {
        int difference = 0;
        if (skillLevel > opponentSkillLevel) {
          if (opponentSkillLevel > 3) {
            difference = opponentSkillLevel - 3;
            if (difference > maxDifference) {
              difference = maxDifference;
            }
          }
        }
        else {
          if (skillLevel > 3) {
            difference = skillLevel - 3;
          }
          if (difference > maxDifference) {
            difference = maxDifference;
          }
        }
        gtw = skillLevel - difference;
      }
      return gtw;
    }

    private PlayerViewModel CreatePlayerViewModel(User player, GameType gameType) {
      var model = new PlayerViewModel();
      model.Id = player.Id;
      model.Name = player.FullName;

      var slQuery = player.SkillLevels.Where(sl => sl.GameType == gameType);
      if (slQuery.Any()) {
        model.SkillLevel = slQuery.First().Value;
      }
      else {
        model.SkillLevel = 0;
      }
      return model;
    }

    public void RegisterRoutes(System.Web.Routing.RouteCollection routes) {
      routes.MapRoute("scoresheet", "scoresheet/{id}", new { Controller = "Scoresheet", Action = "Scoresheet", id = UrlParameter.Optional });
    }
  }
}
