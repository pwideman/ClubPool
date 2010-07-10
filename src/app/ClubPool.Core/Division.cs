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

    public Division(string name, DateTime startingDate) : this() {
      Check.Require(!string.IsNullOrEmpty(name), "Name cannot be null");
      Check.Require(null != startingDate, "Starting date cannot be null");

      Name = name;
      StartingDate = startingDate;
    }

    protected virtual void InitMembers() {
    }

    [NotNull]
    public virtual DateTime StartingDate { get; set; }

    [NotNullNotEmpty]
    public virtual string Name { get; set; }

    public virtual bool CanDelete() {
      return true;
    }

    public virtual Season Season { get; set; }
  }
}
