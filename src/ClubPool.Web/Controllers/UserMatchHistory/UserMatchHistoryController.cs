using System;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.UserMatchHistory
{
  public class UserMatchHistoryController : BaseController
  {
    protected IRepository repository;

    public UserMatchHistoryController(IRepository repository) {
      Arg.NotNull(repository, "repository");
      this.repository = repository;
    }

    [HttpGet]
    [Authorize]
    public ActionResult Index(int id, int? page) {
      var user = repository.Get<User>(id);
      if (null == user) {
        return HttpNotFound();
      }

      // doing this with EF is extremely slow
      var sql = "select * from clubpool.matches where id in (select m.Id from clubpool.matches m, clubpool.matchplayers p where p.Player_Id = @p0 and p.Match_Id = m.Id) order by DatePlayed desc";
      var userMatchesQuery = repository.SqlQuery<Match>(sql, user.Id);
      var viewModel = CreateUserHistoryViewModel(user, userMatchesQuery, page.GetValueOrDefault(1));

      return View(viewModel);
    }

    private UserHistoryViewModel CreateUserHistoryViewModel(User user, IQueryable<Match> matches, int page) {
      var model = new UserHistoryViewModel();
      InitializePagedListViewModel(model, matches, page, 15, (m) => CreateUserHistoryMatchViewModel(m));
      model.Name = user.FullName;
      if (null == matches || matches.Count() == 0) {
        model.HasMatches = false;
      }
      else {
        model.HasMatches = true;
      }
      return model;
    }

    private UserHistoryMatchViewModel CreateUserHistoryMatchViewModel(Match match) {
      var model = new UserHistoryMatchViewModel();
      model.Season = match.Meet.Division.Season.Name;
      var teams = match.Meet.Teams.ToArray();
      model.Team1 = teams[0].Name;
      if (teams.Count() > 1) {
        model.Team2 = teams[1].Name;
      }
      var players = match.Players.ToArray();
      model.Player1 = players[0].Player.FullName;
      model.Player2 = players[1].Player.FullName;
      if (match.IsComplete) {
        model.Winner = match.Winner.FullName;
        if (!match.IsForfeit) {
          model.Date = match.DatePlayed.Value;
          var results = match.Results.Where(r => r.Player == players[0].Player).Single();
          model.Player1Innings = results.Innings;
          model.Player1DefensiveShots = results.DefensiveShots;
          model.Player1Wins = results.Wins;
          results = match.Results.Where(r => r.Player == players[1].Player).Single();
          model.Player2Innings = results.Innings;
          model.Player2DefensiveShots = results.DefensiveShots;
          model.Player2Wins = results.Wins;
        }
      }
      return model;
    }
  }
}
