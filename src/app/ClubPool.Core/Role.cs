using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

using SharpArch.Core;
using SharpArch.Core.DomainModel;

namespace ClubPool.Core
{
  public class Role : Entity
  {
    protected Role() {
      InitMembers();
    }

    public Role(string name)
      : this() {
      Check.Require(!string.IsNullOrEmpty(name), "name cannot be null or empty");

      Name = name;
    }

    protected virtual void InitMembers() {
      Users = new List<User>();
    }

    [DomainSignature]
    [NotNullNotEmpty]
    public virtual string Name { get; set; }

    public virtual IList<User> Users { get; protected set; }
  }
}
