using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Testing;

using ClubPool.SharpArchProviders.Domain;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.NHibernate;

using Tests.ClubPool.Framework.NHibernate.TestDoubles;

namespace Tests.ClubPool.Data.TestDoubles
{
  public static class MockRoleRepositoryFactory
  {
    public static ILinqRepository<Role> CreateMockRoleRepository() {
      return CreateMockRoleRepository(null);
    }

    public static ILinqRepository<Role> CreateMockRoleRepository(IList<Role> roles) {
      if (null == roles) {
        roles = CreateRoles(2);
      }
      ILinqRepository<Role> mockRepository = new MockRepository<Role>(roles);
      return mockRepository;
    }

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
