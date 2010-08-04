using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;
using ClubPool.Core.Contracts;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public abstract class TeamViewModel : ValidatableViewModel
  {
    protected List<PlayerViewModel> availablePlayers;

    protected TeamViewModel() {
      InitMembers();
    }

    protected TeamViewModel(IUserRepository userRepository, Season season) : this() {
      LoadAvailablePlayers(userRepository, season);
    }

    protected TeamViewModel(IUserRepository userRepository, Team team)
      : this(userRepository, team.Division.Season) {
      Name = team.Name;
      Players = team.Players.Select(p => new PlayerViewModel(p)).ToList();
    }

    protected void InitMembers() {
      Players = new List<PlayerViewModel>();
      availablePlayers = new List<PlayerViewModel>();
    }

    private void LoadAvailablePlayers(IUserRepository userRepository, Season season) {
      availablePlayers = userRepository.GetUnassignedUsersForSeason(season).Select(u => new PlayerViewModel(u)).ToList();
    }

    private void RefreshPlayers(IUserRepository userRepository) {
      foreach (var player in Players) {
        var user = userRepository.Get(player.Id);
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
    public void ReInitialize(IUserRepository userRepository, Season season) {
      LoadAvailablePlayers(userRepository, season);
      RefreshPlayers(userRepository);
      // remove my players from available players
      foreach (var player in Players) {
        if (availablePlayers.Contains(player)) {
          availablePlayers.Remove(player);
        }
      }
    }

    [DisplayName("Players:")]
    public IList<PlayerViewModel> Players { get; set; }

    [DisplayName("Available players:")]
    public IEnumerable<PlayerViewModel> AvailablePlayers { get { return availablePlayers; } }

    [DisplayName("Name:")]
    [NotNullNotEmpty]
    public string Name { get; set; }
  }

}
