using System.Collections.Generic;
using System.Linq;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class DetailsViewModel
  {
    public string Name { get; set; }
    public IEnumerable<DetailsPlayerViewModel> Players { get; set; }
    public string Record { get; set; }
    public string Rank { get; set; }

    public DetailsViewModel(Team team) {
      Name = team.Name;
      Record = GetRecord(team);
      var players = new List<DetailsPlayerViewModel>();
      foreach(var player in team.Players) {
        players.Add(new DetailsPlayerViewModel(player));
      }
      Players = players;
    }

    protected string GetRecord(Team team) {
      var winsAndLosses = team.GetWinsAndLosses();
      var wins = winsAndLosses[0];
      var losses = winsAndLosses[1];
      var total = wins + losses;
      double winPct = 0;
      if (total > 0) {
        winPct = (double)wins / (double)total;
      }
      return string.Format("{0} - {1} ({2:.00})", wins, losses, winPct);
    }
  }

  public class DetailsPlayerViewModel
  {
    public string Name { get; set; }
    public int EightBallSkillLevel { get; set; }

    public DetailsPlayerViewModel(User player) {
      Name = player.FullName;
      var skillLevel = player.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).FirstOrDefault();
      if (null != skillLevel) {
        EightBallSkillLevel = skillLevel.Value;
      }
    }
  }
}
