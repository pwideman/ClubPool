using System;
using System.Collections.Generic;

namespace ClubPool.Web.Controllers.Meets
{
  public class DetailsViewModel
  {
    public int Id { get; set; }
    public int ScheduledWeek { get; set; }
    public string ScheduledDate { get; set; }
    public string Team1Name { get; set; }
    public int Team1Id { get; set; }
    public string Team2Name { get; set; }
    public int Team2Id { get; set; }
    public IEnumerable<MatchViewModel> Matches { get; set; }
    public bool AllowUserToEnterResults { get; set; }
  }

  public class MatchPlayerViewModel
  {
    public bool Winner { get; set; }
    public string Record { get; set; }
    public string Innings { get; set; }
    public string DefensiveShots { get; set; }
    public string Wins { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int SkillLevel { get; set; }
    public int GamesToWin { get; set; }
  }

  public class MatchViewModel
  {
    public string DatePlayed { get; set; }
    public string TimePlayed { get; set; }
    public string TimeScheduled { get; set; }
    public MatchPlayerViewModel Player1 { get; set; }
    public MatchPlayerViewModel Player2 { get; set; }
    public bool IsComplete { get; set; }
    public bool IsForfeit { get; set; }
    public int Id { get; set; }
    public string Status { get; set; }
  }
}
