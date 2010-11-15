using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class PreviousUserAccountInfo : Entity
  {
    public virtual User User { get; set; }
    public virtual string Password { get; set; }
    public virtual string Salt { get; set; }
    public virtual int PreviousUserId { get; set; }

    protected PreviousUserAccountInfo() {
    }

    public PreviousUserAccountInfo(User user, string password, string salt, int userId) {
      User = user;
      Password = password;
      Salt = salt;
      PreviousUserId = userId;
    }
  }
}
