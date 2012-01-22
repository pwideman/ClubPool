using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Standings
{
  public class SeasonStandingsViewModel
  {
    public string Name { get; set; }
    public IEnumerable<DivisionStandingsViewModel> Divisions { get; set; }
    public bool HasDivisions { get; set; }
    public IEnumerable<PlayerStandingsViewModel> AllPlayers { get; set; }
  }

  public class DivisionStandingsViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<TeamStandingsViewModel> Teams { get; set; }
    public IEnumerable<PlayerStandingsViewModel> Players { get; set; }
    public bool HasTeams { get; set; }
    public bool HasPlayers { get; set; }
  }

  public class TeamStandingsViewModel : StandingsViewModelBase
  {
    public PlayerStandingsViewModel Player1 { get; set; }
    public PlayerStandingsViewModel Player2 { get; set; }
  }

  public class PlayerStandingsViewModel : StandingsViewModelBase
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
