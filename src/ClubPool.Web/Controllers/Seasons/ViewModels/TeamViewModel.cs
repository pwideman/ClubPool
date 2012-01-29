using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;

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
      var completedMatchesQuery = from meet in team.Division.Meets
                                  from match in meet.Matches
                                  where meet.Teams.Contains(team) && match.IsComplete
                                  select match;
      CanDelete = !completedMatchesQuery.Any();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool CanDelete { get; set; }
    public IEnumerable<PlayerViewModel> Players { get; set; }
  }
}
