using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Meets.ViewModels
{
  public class ScoresheetViewModel
  {
    public int Id { get; set; }
    public int ScheduledWeek { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
    public IEnumerable<ScoresheetMatchViewModel> Matches { get; protected set; }

    public ScoresheetViewModel(Meet meet) {
      Id = meet.Id;
      ScheduledWeek = meet.Week + 1;
      ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7);
      Team1Name = meet.Team1.Name;
      Team2Name = meet.Team2.Name;
      var matches = new List<ScoresheetMatchViewModel>();
      foreach (var match in meet.Matches.Where(m => !m.IsComplete)) {
        matches.Add(new ScoresheetMatchViewModel(match));
      }
      Matches = matches;
    }
  }

  public class ScoresheetMatchViewModel
  {
    public PlayerViewModel Player1;
    public PlayerViewModel Player2;
    public int Id;

    public ScoresheetMatchViewModel(Match match) {
      Id = match.Id;

      var team = match.Meet.Teams.Where(t => t.Players.Contains(match.Player1)).First();
      Player1 = new PlayerViewModel(match.Player1, team);

      team = match.Meet.Teams.Where(t => t.Players.Contains(match.Player2)).First();
      Player2 = new PlayerViewModel(match.Player2, team);

      Player1.GamesToWin = CalculateGamesToWin(Player1.SkillLevel, Player2.SkillLevel);
      Player2.GamesToWin = CalculateGamesToWin(Player2.SkillLevel, Player1.SkillLevel);
    }

    private int CalculateGamesToWin(int skillLevel, int opponentSkillLevel) {
      int gtw = 0;
      int maxDifference = 1; // number of games to reduce skill level by
      // compute GTW
      if (0 == skillLevel || 0 == opponentSkillLevel) {
        gtw = 4;
      }
      else {
        int difference = 0;
        if (skillLevel > opponentSkillLevel) {
          if (opponentSkillLevel > 3) {
            difference = opponentSkillLevel - 3;
            if (difference > maxDifference) {
              difference = maxDifference;
            }
          }
        }
        else {
          if (skillLevel > 3) {
            difference = skillLevel - 3;
          }
          if (difference > maxDifference) {
            difference = maxDifference;
          }
        }
        gtw = skillLevel - difference;
      }
      return gtw;
    }
  }

}
