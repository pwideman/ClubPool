using System;
using System.Reflection;

using ClubPool.Web.Models;

namespace ClubPool.Testing
{
  public static class EntityVersionSetter
  {
    public static void SetVersionOf(VersionedEntity entity, int version) {
      PropertyInfo versionProperty = entity.GetType().GetProperty("Version",
          BindingFlags.Public | BindingFlags.Instance);

      versionProperty.SetValue(entity, BitConverter.GetBytes(version), null);
    }

    public static VersionedEntity SetVersionTo(this VersionedEntity entity, int version) {
      SetVersionOf(entity, version);
      return entity;
    }
  }
}
