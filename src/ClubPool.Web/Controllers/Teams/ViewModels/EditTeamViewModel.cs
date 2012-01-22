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

    public EditTeamViewModel(IRepository repository, Team team, bool selectCurrentPlayers = true)
      : base(repository, team, selectCurrentPlayers) {
      Id = team.Id;
      Version = team.EncodedVersion;
    }

    [Required]
    public string Version { get; set; }

    [Min(1)]
    public int Id { get; set; }

    public int[] SelectedPlayers { get; set; }
  }
}
