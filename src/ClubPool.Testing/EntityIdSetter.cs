using System.Reflection;

using ClubPool.Web.Models;

namespace ClubPool.Testing
{
  public static class EntityIdSetter
  {
    public static void SetIdOf(Entity entity, int id)
    {
      PropertyInfo idProperty = entity.GetType().GetProperty("Id",
          BindingFlags.Public | BindingFlags.Instance);
      idProperty.SetValue(entity, id, null);
    }

    public static Entity SetIdTo(this Entity entity, int id)
    {
      SetIdOf(entity, id);
      return entity;
    }
  }
}
