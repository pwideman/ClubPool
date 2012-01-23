using System;
using System.Collections.Generic;

using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Dashboard.ViewModels
{
  public class IndexViewModel
  {
    public bool UserIsAdmin { get; set; }
    public string UserFullName { get; set; }
    public bool HasCurrentSeasonStats { get; set; }
    public StatsViewModel CurrentSeasonStats { get; set; }
    public bool HasLastMeetStats { get; set; }
    public LastMeetViewModel LastMeetStats { get; set; }
    public bool HasSeasonResults { get; set; }
    public IEnumerable<SeasonResultViewModel> SeasonResults { get; set; }
    public SkillLevelCalculationViewModel SkillLevelCalculation { get; set; }
  }

  public class StatsViewModel
  {
    public int SkillLevel { get; set; }
    public string PersonalRecord { get; set; }
    public string TeamRecord { get; set; }
    public string TeamName { get; set; }
    public int TeamId { get; set; }
    public string TeammateName { get; set; }
    public int TeammateId { get; set; }
  }

  public class LastMeetViewModel
  {
    public string OpponentTeam { get; set; }
    public IEnumerable<LastMatchViewModel> Matches { get; set; }
  }

  public class LastMatchViewModel
  {
    public IEnumerable<MatchResultViewModel> Results { get; set; }
  }

  public class SeasonResultViewModel
  {
    public string Team { get; set; }
    public string Player { get; set; }
    public bool Win { get; set; }
  }

}
