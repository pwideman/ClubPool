using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace ClubPool.Data.NHibernateMaps.Conventions
{
  public class HasManyConvention : IHasManyConvention
  {
    public void Apply(IOneToManyCollectionInstance instance) {
      instance.Key.Column(instance.EntityType.Name + "Id");
      instance.Cascade.AllDeleteOrphan();
      instance.Access.ReadOnlyPropertyThroughCamelCaseField();
    }
  }
}
