using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class TeamViewModel
  {
    public TeamViewModel() {
      Players = new List<PlayerViewModel>();
    }

    public TeamViewModel(Team team) {
      Id = team.Id;
      Name = team.Name;
      Players = team.Players.Select(p => new PlayerViewModel(p)).ToList();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<PlayerViewModel> Players { get; set; }
  }
}
