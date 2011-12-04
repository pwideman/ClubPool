using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class SkillLevel : Entity, IEntityWithVersion
  {
    public virtual GameType GameType { get; set; }
    public virtual int Value { get; set; }
    public virtual int Version { get; set; }
    public virtual User User { get; set; }

    protected SkillLevel() {
    }

    public SkillLevel(User user, GameType gameType, int value) {
      Check.Require(null != user, "player cannot be null");
      Check.Require(value >= 0, "value must be >= 0");

      User = user;
      GameType = gameType;
      Value = value;
    }
  }
}
