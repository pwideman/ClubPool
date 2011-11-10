using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Shared.ViewModels
{
  public class SkillLevelCalculationViewModel
  {
    public IEnumerable<SkillLevelMatchResultViewModel> SkillLevelMatchResults { get; set; }
    public int TotalInnings { get; set; }
    public int TotalDefensiveShots { get; set; }
    public int TotalNetInnings { get; set; }
    public int TotalWins { get; set; }
    public int TotalCulledInnings { get; set; }
    public int TotalCulledDefensiveShots { get; set; }
    public int TotalCulledNetInnings { get; set; }
    public int TotalCulledWins { get; set; }
    public bool HasSkillLevel { get; set; }
    public double TotalIG { get; set; }
    public double CulledIG { get; set; }
    public int EightBallSkillLevel { get; set; }

    public SkillLevelCalculationViewModel(User player, IRepository repository) {
      var sl = player.SkillLevels.Where(s => s.GameType == GameType.EightBall).SingleOrDefault();
      if (null != sl) {
        EightBallSkillLevel = sl.Value;
        var matchResults = player.GetMatchResultsUsedInSkillLevelCalculation(GameType.EightBall, repository);
        HasSkillLevel = true;
        var culledMatchResults = player.CullTopMatchResults(matchResults);
        var results = new List<SkillLevelMatchResultViewModel>();
        foreach (var result in matchResults) {
          var resultvm = new SkillLevelMatchResultViewModel(result, player);
          results.Add(resultvm);
          if (culledMatchResults.Contains(result)) {
            resultvm.Included = true;
            TotalCulledInnings += resultvm.Innings;
            TotalCulledDefensiveShots += resultvm.DefensiveShots;
            TotalCulledNetInnings += resultvm.NetInnings;
            TotalCulledWins += resultvm.Wins;
          }
          TotalInnings += resultvm.Innings;
          TotalDefensiveShots += resultvm.DefensiveShots;
          TotalNetInnings += resultvm.NetInnings;
          TotalWins += resultvm.Wins;
        }
        SkillLevelMatchResults = results.OrderByDescending(r => r.Date);
        TotalIG = (double)TotalNetInnings / (double)TotalWins;
        CulledIG = (double)TotalCulledNetInnings / (double)TotalCulledWins;
      }
    }
  }

  public class SkillLevelMatchResultViewModel : MatchResultViewModel
  {
    public string Team { get; set; }
    public DateTime Date { get; set; }
    public bool Included { get; set; }
    public int NetInnings { get; set; }

    public SkillLevelMatchResultViewModel(MatchResult matchResult, User player)
      : base(matchResult) {
      var match = matchResult.Match;
      Date = match.DatePlayed.Value;
      Team = match.Meet.Teams.Where(t => !t.Players.Contains(player)).Single().Name;
      NetInnings = Innings - DefensiveShots;
      // for this we display our opponent
      Player = match.Players.Where(p => p.Player != player).Single().Player.FullName;
    }
  }

  public class MatchResultViewModel
  {
    public string Player { get; set; }
    public int Innings { get; set; }
    public int DefensiveShots { get; set; }
    public int Wins { get; set; }
    public bool Winner { get; set; }

    public MatchResultViewModel() {
    }

    public MatchResultViewModel(MatchResult result) {
      Player = result.Player.FullName;
      Innings = result.Innings;
      DefensiveShots = result.DefensiveShots;
      Wins = result.Wins;
    }
  }

}
