using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions
{
  public class ReferenceConvention : IReferenceConvention
  {
    public void Apply(FluentNHibernate.Conventions.Instances.IManyToOneInstance instance) {
      instance.Column(instance.Property.Name + "Id");
    }
  }
}
