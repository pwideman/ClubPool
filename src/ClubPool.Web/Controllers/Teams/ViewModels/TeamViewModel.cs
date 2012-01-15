using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public abstract class TeamViewModel
  {
    protected TeamViewModel() {
      InitMembers();
    }

    protected TeamViewModel(IRepository repository, Season season) : this() {
      LoadAvailablePlayers(repository, season);
    }

    protected TeamViewModel(IRepository repository, Team team, bool selectCurrentPlayers = true)
      : this(repository, team.Division.Season) {
      Name = team.Name;
      // get team players and merge with available players
      var teamPlayers = team.Players.Select(p => new PlayerViewModel { Player = p, IsSelected = selectCurrentPlayers }).ToList();
      var players = Players.ToList();
      players.InsertRange(0, teamPlayers);
      Players = players;
      SchedulePriority = team.SchedulePriority;
    }

    protected void InitMembers() {
      Players = new List<PlayerViewModel>();
    }

    private void LoadAvailablePlayers(IRepository repository, Season season) {
      var sql = "select * from clubpool.Users where IsApproved = 1 and id not in " +
        "(select distinct u.id from clubpool.Users u, clubpool.TeamsUsers tp, clubpool.Teams t, clubpool.Divisions d, clubpool.Seasons s where " +
        "u.Id = tp.User_Id and s.id in (select Season_Id from clubpool.Divisions where Id in " +
        "(select Division_Id from clubpool.Teams where id = tp.Team_Id)) and s.Id = @p0)" +
        "order by LastName, FirstName";

      Players = repository.SqlQuery<User>(sql, season.Id).Select(u => new PlayerViewModel { Player = u }).ToList();
    }

    [DisplayName("Players:")]
    public IEnumerable<PlayerViewModel> Players { get; set; }

    [DisplayName("Name:")]
    [Required(ErrorMessage="Required")]
    public string Name { get; set; }

    [DisplayName("Schedule priority:")]
    [Required(ErrorMessage="Required")]
    [Min(0, ErrorMessage="Schedule priority must be a number greater than or equal to zero")]
    public int SchedulePriority { get; set; }

  }

}
