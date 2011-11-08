using ClubPool.Web.Infrastructure;
using ClubPool.Core;

namespace ClubPool.Web.Models
{
  public class SkillLevel : VersionedEntity
  {
    public virtual GameType GameType { get; set; }
    public virtual int Value { get; set; }
    public virtual User User { get; set; }

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
