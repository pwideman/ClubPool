using System.Linq;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class EditTeamViewModel : TeamViewModel
  {
    public EditTeamViewModel()
      : base() {
    }

    public EditTeamViewModel(IRepository repository, Team team)
      : base(repository, team) {
      Id = team.Id;
      Version = team.EncodedVersion;
    }

    // hide the base method
    private new void ReInitialize(IRepository repository, Season season) {
    }

    public void ReInitialize(IRepository repository, Team team) {
      base.ReInitialize(repository, team.Division.Season);
      // add the team's players to the available players if they aren't in my players
      foreach (var player in team.Players.Select(p => new PlayerViewModel { Player = p })) {
        if (!Players.Contains(player)) {
          availablePlayers.Add(player);
        }
      }
    }

    [Required]
    public string Version { get; set; }

    [Min(1)]
    public int Id { get; set; }
  }
}
