using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core.DomainModel;

namespace Tests.ClubPool.SharpArchProviders
{
  public static class EntityIdGenerator
  {
    public static IdT FindNextId<IdT>(this IList<EntityWithTypedId<IdT>> entities) {
      var idInUse = false;
      object id = null;
      var index = 1;
      do {
        id = GetIdForIndex<IdT>(index);
        idInUse = false;
        foreach(var entity in entities) {
          if (entity.Id.Equals(id)) {
            idInUse = true;
            index++;
            break;
          }
        }
      } while (idInUse);
      return (IdT)id;
    }

    private static IdT GetIdForIndex<IdT>(int index) {
      object id = null;
      // must add a special case here for each type of entity Id in our domain
      if (typeof(IdT) == typeof(int)) {
        id = index;
      }
      return (IdT)id;
    }
  }
}
