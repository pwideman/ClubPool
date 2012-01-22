using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.CurrentSeason.ViewModels
{
  public class CurrentSeasonStandingsViewModel
  {
    public string Name { get; set; }
    public IEnumerable<StandingsDivisionViewModel> Divisions { get; set; }
    public bool HasDivisions { get; set; }
    public IEnumerable<StandingsPlayerViewModel> AllPlayers { get; set; }
  }

  public class StandingsDivisionViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<StandingsTeamViewModel> Teams { get; set; }
    public IEnumerable<StandingsPlayerViewModel> Players { get; set; }
    public bool HasTeams { get; set; }
    public bool HasPlayers { get; set; }
  }

  public class StandingsTeamViewModel : StandingsViewModelBase
  {
    public StandingsPlayerViewModel Player1 { get; set; }
    public StandingsPlayerViewModel Player2 { get; set; }
  }

  public class StandingsPlayerViewModel : StandingsViewModelBase
  {
    public int SkillLevel { get; set; }
  }

  public abstract class StandingsViewModelBase
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinPercentage { get; set; }
    public string Rank { get; set; }
    public bool Highlight { get; set; }
  }
}
