using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Scoresheet
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
      var teams = meet.Teams.ToArray();
      var team1 = teams[0];
      var team2 = teams[1];
      Team1Name = team1.Name;
      Team2Name = team2.Name;
      var matches = new List<ScoresheetMatchViewModel>();
      foreach (var match in meet.Matches) {
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

      var gameType = match.Meet.Division.Season.GameType;
      var players = match.Players.ToArray();
      Player1 = new PlayerViewModel(players[0].Player, gameType);
      Player2 = new PlayerViewModel(players[1].Player, gameType);

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

  public class PlayerViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int SkillLevel { get; set; }
    public int GamesToWin { get; set; }

    public PlayerViewModel(User player, GameType gameType) {
      Id = player.Id;
      Name = player.FullName;

      var slQuery = player.SkillLevels.Where(sl => sl.GameType == gameType);
      if (slQuery.Any()) {
        SkillLevel = slQuery.First().Value;
      }
      else {
        SkillLevel = 0;
      }
    }
  }
}
