using System;
using System.Collections.Generic;

namespace ClubPool.Web.Controllers.Scoresheet
{
  public class ScoresheetViewModel
  {
    public int Id { get; set; }
    public int ScheduledWeek { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
    public IEnumerable<ScoresheetMatchViewModel> Matches { get; set; }
  }

  public class ScoresheetMatchViewModel
  {
    public PlayerViewModel Player1;
    public PlayerViewModel Player2;
    public int Id;
  }

  public class PlayerViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int SkillLevel { get; set; }
    public int GamesToWin { get; set; }
  }
}
