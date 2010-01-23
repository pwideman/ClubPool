using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.NHibernate;
using ClubPool.SharpArchProviders;
using ClubPool.SharpArchProviders.Domain;

namespace Tests.ClubPool.SharpArchProviders.TestDoubles
{
  public class TestSharpArchMembershipProvider : SharpArchMembershipProvider
  {
    public ILinqRepository<User> UserRepository {
      get { return base.userRepository; }
      set { base.userRepository = value; }
    }

    public new int SALT_SIZE {
      get { return 16; }
    }

    public new string EncodePassword(string password, string salt) {
      return base.EncodePassword(password, salt);
    }

    public new string GenerateSalt(int size) {
      return base.GenerateSalt(size);
    }
  }
}
