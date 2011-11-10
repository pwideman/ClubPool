using System.ComponentModel.DataAnnotations;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class MatchPlayer : Entity
  {
    [Required]
    public virtual Match Match { get; set; }
    [Required]
    public virtual User Player { get; set; }
    // Team should be required, but SqlServerCe complains about a cyclical reference
    //[Required]
    public virtual Team Team { get; set; }

    protected MatchPlayer() {
    }

    public MatchPlayer(User player, Team team) {
      Arg.NotNull(player, "player");
      Arg.NotNull(team, "team");

      Player = player;
      Team = team;
    }
  }
}
