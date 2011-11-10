using System.ComponentModel.DataAnnotations;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class SkillLevel : VersionedEntity
  {
    public int GameTypeValue { get; set; }
    public int Value { get; set; }
    [Required]
    public virtual User User { get; set; }

    public GameType GameType {
      get {
        return (GameType)GameTypeValue;
      }
      set {
        GameTypeValue = (int)value;
      }
    }

    protected SkillLevel() {
    }

    public SkillLevel(User user, GameType gameType, int value) {
      Arg.NotNull(user, "user");
      Arg.Require(value >= 0, "value must be >= 0");

      User = user;
      GameType = gameType;
      Value = value;
    }
  }
}
