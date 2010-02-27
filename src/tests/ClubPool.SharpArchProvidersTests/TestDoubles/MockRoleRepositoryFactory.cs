using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Testing;

using ClubPool.Framework.Extensions;
using ClubPool.Framework.NHibernate;
using ClubPool.SharpArchProviders.Domain;

namespace Tests.ClubPool.SharpArchProviders.TestDoubles
{
  public static class MockRoleRepositoryFactory
  {
    public static Role CreateTransientRole(string name) {
      if (name.IsNullOrEmptyOrBlank()) {
        name = "role";
      }
      return new Role(name);
    }

    public static IList<Role> CreateRoles(int count) {
      var roles = new List<Role>();
      for (var i = 0; i < count; i++) {
        roles.Add(CreateRole(i));
      }
      return roles;
    }

    public static Role CreateRole(int id) {
      var name = "role" + id.ToString();
      var role = CreateTransientRole(name);
      role.SetIdTo(id);
      return role;
    }
  }
}
