using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Testing;

using ClubPool.Core;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.NHibernate;

using Tests.ClubPool.Framework.NHibernate.TestDoubles;

namespace Tests.ClubPool.Data.TestDoubles
{
  public static class MockUserRepositoryFactory
  {

    public static ILinqRepository<User> CreateMockUserRepository() {
      return CreateMockUserRepository(null);
    }

    public static ILinqRepository<User> CreateMockUserRepository(IList<User> users) {
      if (null == users) {
        users = CreateUsers(4);
      }
      ILinqRepository<User> mockedRepository = new MockRepository<User>(users);
      return mockedRepository;
    }

    public static User CreateTransientUser(string name) {
      if (name.IsNullOrEmptyOrBlank()) {
        name = "user";
      }
      var user = new User(name, name, name, name, name + "@email.com");
      user.PasswordSalt = name;
      return user;
    }

    public static List<User> CreateUsers(int count) {
      List<User> users = new List<User>();
      for (var i = 0; i < count; i++) {
        users.Add(CreateUser(i));
      }
      return users;
    }

    public static User CreateUser(int id) {
      var name = "user" + id.ToString();
      var user = CreateTransientUser(name);
      user.SetIdTo(id);
      return user;
    }
  }
}
