using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

using SharpArch.Core;
using SharpArch.Core.DomainModel;

namespace ClubPool.SharpArchProviders.Domain
{
  public class Role : Entity
  {
    public Role() {
      InitMembers();
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
