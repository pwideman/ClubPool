using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;
using SharpArch.Core.DomainModel;

namespace ClubPool.Web.Controllers
{
  public abstract class ViewModelBase
  {
  }

  public abstract class EntityViewModelBase
  {
    public EntityViewModelBase() {
    }

    public EntityViewModelBase(Entity entity) : this() {
      Id = entity.Id;
    }

    [Min(1)]
    public int Id { get; set; }
  }
}
