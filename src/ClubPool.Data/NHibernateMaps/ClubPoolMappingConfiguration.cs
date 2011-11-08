using System;
using System.Linq;

using FluentNHibernate;
using FluentNHibernate.Automapping;

using SharpArch.Core.DomainModel;

namespace ClubPool.Data.NHibernateMaps
{
  public class ClubPoolMappingConfiguration : DefaultAutomappingConfiguration
  {
    public override bool ShouldMap(Type type) {
      return type.GetInterfaces().Any(x =>
           x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntityWithTypedId<>));
    }

    //public override bool ShouldMap(Member member) {
    //  return base.ShouldMap(member) && member.CanWrite;
    //}

    public override bool AbstractClassIsLayerSupertype(Type type) {
      return type == typeof(EntityWithTypedId<>) || type == typeof(Entity);
    }

    public override bool IsId(Member member) {
      return member.Name == "Id";
    }
  }
}
