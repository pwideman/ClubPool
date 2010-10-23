using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace ClubPool.Data.NHibernateMaps.Conventions
{
  public class ReferenceConvention : IReferenceConvention
  {
    public void Apply(FluentNHibernate.Conventions.Instances.IManyToOneInstance instance) {
      instance.Column(instance.Property.Name + "Id");
      instance.Cascade.SaveUpdate();
    }
  }
}
