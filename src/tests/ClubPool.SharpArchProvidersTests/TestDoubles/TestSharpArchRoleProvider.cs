using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.NHibernate;
using ClubPool.SharpArchProviders;
using ClubPool.SharpArchProviders.Domain;

namespace Tests.ClubPool.SharpArchProviders.TestDoubles
{
  public class TestSharpArchRoleProvider : SharpArchRoleProvider
  {
    public ILinqRepository<Role> RoleRepository {
      get { return base.roleRepository; }
      set { base.roleRepository = value; }
    }

    public ILinqRepository<User> UserRepository {
      get { return base.userRepository; }
      set { base.userRepository = value; }
    }
  }
}
