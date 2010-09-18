using System.Reflection;

using SharpArch.Core;

using ClubPool.Core;

namespace ClubPool.Testing
{
  /// <summary>
  /// For better data integrity, it is imperitive that the <see cref="Entity.Version"/>
  /// property is read-only and set only by the ORM.  With that said, some unit tests need 
  /// Version set to a particular value; therefore, this utility enables that ability.  This class should 
  /// never be used outside of the testing project.
  /// 
  /// This class is basically a copy of the SharpArch EntityIdSetter.
  /// </summary>
  public static class EntityVersionSetter
  {
    /// <summary>
    /// Uses reflection to set the Version of an entity.
    /// </summary>
    public static void SetVersionOf(IEntityWithVersion entity, int version) {
      // Set the data property reflectively
      PropertyInfo versionProperty = entity.GetType().GetProperty("Version",
          BindingFlags.Public | BindingFlags.Instance);

      Check.Ensure(versionProperty != null, "Version property could not be found");

      versionProperty.SetValue(entity, version, null);
    }

    /// <summary>
    /// Uses reflection to set the Version of an entity.
    /// </summary>
    public static IEntityWithVersion SetVersionTo(this IEntityWithVersion entity, int version) {
      SetVersionOf(entity, version);
      return entity;
    }
  }
}
