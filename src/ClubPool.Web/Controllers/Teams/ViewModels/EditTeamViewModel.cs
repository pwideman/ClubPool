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
  public class EditTeamViewModel : TeamViewModel
  {
    public EditTeamViewModel()
      : base() {
    }

    public EditTeamViewModel(IUserRepository userRepository, Team team)
      : base(userRepository, team) {
      Id = team.Id;
      Version = team.Version;
    }

    // hide the base method
    private new void ReInitialize(IUserRepository userRepository, Season season) {
    }

    public void ReInitialize(IUserRepository userRepository, Team team) {
      base.ReInitialize(userRepository, team.Division.Season);
      // add the team's players to the available players if they aren't in my players
      foreach(var player in team.Players.Select(p => new PlayerViewModel(p))) {
        if (!Players.Contains(player)) {
          availablePlayers.Add(player);
        }
      }
    }

    [Min(1)]
    public int Version { get; set; }

    [Min(1)]
    public int Id { get; set; }
  }
}
