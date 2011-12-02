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
    protected List<PlayerViewModel> availablePlayers;

    protected TeamViewModel() {
      InitMembers();
    }

    protected TeamViewModel(IRepository repository, Season season) : this() {
      LoadAvailablePlayers(repository, season);
    }

    protected TeamViewModel(IRepository repository, Team team)
      : this(repository, team.Division.Season) {
      Name = team.Name;
      Players = team.Players.Select(p => new PlayerViewModel { Player = p }).ToList();
      SchedulePriority = team.SchedulePriority;
    }

    protected void InitMembers() {
      Players = new List<PlayerViewModel>();
      availablePlayers = new List<PlayerViewModel>();
    }

    private void LoadAvailablePlayers(IRepository repository, Season season) {
      var sql = "select * from Users where IsApproved = 1 and id not in " +
        "(select distinct u.id from Users u, TeamsUsers tp, Teams t, Divisions d, Seasons s where " +
        "u.Id = tp.User_Id and s.id in (select Season_Id from Divisions where Id in " +
        "(select Division_Id from Teams where id = tp.Team_Id)) and s.Id = @p0)" +
        "order by LastName, FirstName";

      availablePlayers = repository.SqlQuery<User>(sql, season.Id).Select(u => new PlayerViewModel { Player = u }).ToList();
    }

    private void RefreshPlayers(IRepository repository) {
      foreach (var player in Players) {
        var user = repository.Get<User>(player.Id);
        player.Name = user.FullName;
        player.Username = user.Username;
        player.Email = user.Email;
      }
    }

    /// <summary>
    /// Refreshes a view model that has been posted back from a form
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="season"></param>
    public void ReInitialize(IRepository repository, Season season) {
      LoadAvailablePlayers(repository, season);
      RefreshPlayers(repository);
      // remove my players from available players
      foreach (var player in Players) {
        if (availablePlayers.Contains(player)) {
          availablePlayers.Remove(player);
        }
      }
    }

    [DisplayName("Players:")]
    public IEnumerable<PlayerViewModel> Players { get; set; }

    [DisplayName("Available players:")]
    public IEnumerable<PlayerViewModel> AvailablePlayers { get { return availablePlayers; } }

    [DisplayName("Name:")]
    [Required(ErrorMessage="Required")]
    public string Name { get; set; }

    [DisplayName("Schedule priority:")]
    [Min(0)]
    public int SchedulePriority { get; set; }

  }

}
