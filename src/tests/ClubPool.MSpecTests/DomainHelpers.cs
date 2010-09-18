﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Testing;

using ClubPool.Core;
using ClubPool.Testing;

namespace ClubPool.MSpecTests
{
  public static class DomainHelpers
  {
    public static List<User> GetUsers(int startingId, int count) {
      var users = new List<User>();
      for (var i = 0; i < count; i++) {
        var id = startingId + i;
        var user = new User("user" + id.ToString(), "pass", "first" + id.ToString(), "last" + id.ToString(), "user" + id.ToString() + "@email.com");
        user.SetIdTo(id);
        user.SetVersionTo(1);
        users.Add(user);
      }
      return users;
    }

    public static List<User> GetUsers(int count) {
      return GetUsers(1, count);
    }

  }
}
