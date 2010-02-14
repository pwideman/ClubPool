using System;
using System.Collections.Generic;

using NHibernate.Validator.Constraints;
using SharpArch.Core.NHibernateValidator;
using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.SharpArchProviders.Domain
{
  [HasUniqueDomainSignature(Message="A user already exists with this username")]
  public class User : Entity
  {
    public User() {
      InitMembers();
    }

    protected virtual void InitMembers() {
      Roles = new List<Role>();
    }

    [DomainSignature]
    [NotNullNotEmpty]
    public virtual string Username { get; set; }

    [NotNullNotEmpty]
    public virtual string Password { get; set; }

    public virtual string PasswordSalt { get; set; }

    [Email]
    public virtual string Email { get; set; }

    public virtual IList<Role> Roles { get; protected set; }
  }
}
