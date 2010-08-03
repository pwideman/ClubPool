﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
      users = new List<User>();
    }

    [DomainSignature]
    public virtual string Name { get; set; }

    protected IList<User> users;

    public virtual IEnumerable<User> Users { get { return users; } }

  }
}
