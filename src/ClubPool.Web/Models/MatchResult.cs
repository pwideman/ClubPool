using System;
using System.ComponentModel.DataAnnotations;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class MatchResult : VersionedEntity
  {
    [Required]
    public virtual Match Match { get; set; }
    [Required]
    public virtual User Player { get; set; }
    public int Innings { get; set; }
    public int DefensiveShots { get; set; }
    public int Wins { get; set; }

    protected MatchResult() {
    }

    public MatchResult(User player, int innings, int defensiveShots, int wins) {
      Arg.NotNull(player, "player");
      Arg.Require(innings >= 0, "innings must be >= 0");
      Arg.Require(defensiveShots >= 0, "defensiveShots must be >= 0");
      Arg.Require(wins >= 0, "wins must be >= 0");

      Player = player;
      Innings = innings;
      DefensiveShots = defensiveShots;
      Wins = wins;
    }
  }
}
