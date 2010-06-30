using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

using SharpArch.Core;
using SharpArch.Core.DomainModel;

namespace ClubPool.Core
{
  public class Season : Entity
  {
    protected Season() {
      InitMembers();
    }

    public Season(string name)
      : this() {
      Check.Require(!string.IsNullOrEmpty(name), "name cannot be null or empty");

      Name = name;
    }

    protected virtual void InitMembers() {
    }

    [DomainSignature]
    [NotNullNotEmpty]
    public virtual string Name { get; set; }

    public virtual bool IsActive { get; set; }

    public virtual bool CanDelete() {
      return !IsActive;
    }
  }
}
