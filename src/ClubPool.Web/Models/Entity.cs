using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubPool.Web.Models
{
  public class Entity
  {
    public virtual int Id { get; protected set; }

    public bool IsTransient() {
      return Id.Equals(default(int));
    }
  }

  public class VersionedEntity : Entity
  {
    public virtual int Version { get; protected set; }
  }
}