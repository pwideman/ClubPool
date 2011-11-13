using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public abstract class TeamViewModel : ValidatableViewModel
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
      var unavailableUsersQuery = from d in season.Divisions
                                  from t in d.Teams
                                  from u in t.Players
                                  select u.Id;


      availablePlayers = (from u in repository.All<User>().Where(u => !unavailableUsersQuery.Contains(u.Id))
                          orderby u.LastName, u.FirstName
                          select new PlayerViewModel { Player = u }).ToList();
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
    [NotNullNotEmpty]
    public string Name { get; set; }

    [DisplayName("Schedule priority:")]
    [Min(0)]
    public int SchedulePriority { get; set; }

  }

}
