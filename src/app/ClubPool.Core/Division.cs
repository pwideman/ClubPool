using System;
using System.Collections.Generic;

using NHibernate.Validator.Constraints;
using SharpArch.Core.NHibernateValidator;
using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class Division : Entity
  {
    protected Division() {
      InitMembers();
    }

    protected virtual void InitMembers() {
    }

    [NotNull]
    public virtual DateTime StartingDate { get; set; }

    [NotNull]
    public virtual TimeSpan Periodicity { get; set; }

    [NotNullNotEmpty]
    public virtual string Name { get; set; }

    public virtual bool CanDelete() {
      return true;
    }
  }
}
