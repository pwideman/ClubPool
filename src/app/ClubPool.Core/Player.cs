using System;
using System.Collections.Generic;

using NHibernate.Validator.Constraints;

using SharpArch.Core;
using SharpArch.Core.DomainModel;

using ClubPool.SharpArchProviders.Domain;

namespace ClubPool.Core
{
  public class Player : Entity
  {
    public Player() { }

    [DomainSignature]
    public virtual User User { get; set; }
  }
}
