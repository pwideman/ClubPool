using System;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;

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

    public override int GetHashCode() {
      return Id;
    }

    public override bool Equals(object obj) {
      var viewModel = obj as EntityViewModelBase;
      if (null == viewModel) {
        return false;
      }
      else {
        return Id == viewModel.Id;
      }
    }

    [Min(1)]
    public int Id { get; set; }
  }
}
