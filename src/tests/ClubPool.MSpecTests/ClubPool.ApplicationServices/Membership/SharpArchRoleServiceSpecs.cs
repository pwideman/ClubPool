using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino.Mocks;
using Machine.Specifications;

using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Queries;
using ClubPool.ApplicationServices.Membership;
using ClubPool.Testing.ApplicationServices.Membership;

namespace ClubPool.MSpecTests.ClubPool.ApplicationServices.Membership
{
  public class specification_for_SharpArchRoleService
  {
    protected static SharpArchRoleService service;
    protected static ILinqRepository<Role> roleRepository;
    protected static ILinqRepository<User> userRepository;

    Establish context = () => {
      userRepository = MockRepository.GenerateStub<ILinqRepository<User>>();
      roleRepository = MockRepository.GenerateStub<ILinqRepository<Role>>();

      service = new SharpArchRoleService(roleRepository, userRepository);
    };
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_to_get_roles_for_user_with_an_invalid_username : specification_for_SharpArchRoleService
  {
    static Exception theException;

    Establish context = () => {
      userRepository.Expect(r => r.FindOne(null)).IgnoreArguments().Return(null);
    };

    Because of = () => {
      try {
        service.GetRolesForUser("test");
      }
      catch (Exception e) {
        theException = e;
      }
    };

    It should_throw_an_ArgumentException = () => {
      theException.ShouldNotBeNull();
      theException.ShouldBeOfType<ArgumentException>();
    };
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_to_get_roles_for_a_user_assigned_to_no_roles : specification_for_SharpArchRoleService
  {
    static User theUser;
    static string username = "test";
    static string[] result;

    Establish context = () => {
      theUser = new User(username, "test", "test", "test", "test");
      userRepository.Expect(r => r.FindOne(null)).IgnoreArguments().Return(theUser);
      roleRepository.Expect(r => r.GetAll()).Return(new List<Role>().AsQueryable());
    };

    Because of = () => {
      result = service.GetRolesForUser(username);
    };

    It should_return_an_empty_array = () => {
      result.ShouldNotBeNull();
      result.Length.ShouldEqual(0);
    };
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_to_get_roles_for_a_user_with_roles : specification_for_SharpArchRoleService
  {
    static User theUser;
    static string username = "test";
    static string[] result;
    static List<Role> roles;
    static int numRoles = 2;

    Establish context = () => {
      theUser = new User(username, "test", "test", "test", "test");
      roles = new List<Role>();
      for (int i = 0; i < 2; i++) {
        var role = new Role("role" + i);
        role.Users.Add(theUser);
        roles.Add(role);
      }
      roles.Add(new Role("bad"));
      userRepository.Expect(r => r.FindOne(null)).IgnoreArguments().Return(theUser);
      roleRepository.Expect(r => r.GetAll()).Return(roles.AsQueryable());
    };

    Because of = () => {
      result = service.GetRolesForUser(username);
    };

    It should_return_all_roles_that_contain_the_user = () => {
      result.Length.ShouldEqual(numRoles);
      result[0].ShouldEqual(roles[0].Name);
      result[1].ShouldEqual(roles[1].Name);
    };
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_if_user_is_in_invalid_role : specification_for_SharpArchRoleService
  {
    static bool result;

    Because of = () => result = service.IsUserInRole("test", "test");

    It should_return_false = () => result.ShouldBeFalse();
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_if_invalid_user_is_in_role : specification_for_SharpArchRoleService
  {
    static bool result;
    static string roleName = "test";

    Establish context = () => {
      var role = new Role(roleName);
      roleRepository.Expect(r => r.FindOne(null)).IgnoreArguments().Return(role);
    };

    Because of = () => result = service.IsUserInRole("test", roleName);

    It should_return_false = () => result.ShouldBeFalse();
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_if_user_is_in_role : specification_for_SharpArchRoleService
  {
    static bool result;
    static string roleName = "role";
    static string username = "user";

    Establish context = () => {
      var user = new User(username, "test", "test", "test", "test");
      var role = new Role(roleName);
      role.Users.Add(user);
      roleRepository.Expect(r => r.FindOne(null)).IgnoreArguments().Return(role);
    };

    Because of = () => result = service.IsUserInRole(username, roleName);

    It should_return_true = () => result.ShouldBeTrue();
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_for_users_in_invalid_role : specification_for_SharpArchRoleService
  {
    static string[] result;
    static string roleName = "role";
    static Exception theException;

    Because of = () => {
      try {
        result = service.GetUsersInRole(roleName);
      }
      catch(Exception e) {
        theException = e;
      }
    };

    It should_throw_an_ArgumentException = () => {
      theException.ShouldNotBeNull();
      theException.ShouldBeOfType<ArgumentException>();
    };
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_for_users_in_role : specification_for_SharpArchRoleService
  {
    static string[] result;
    static string roleName = "role";
    static int numUsers = 2;
    static Role role;

    Establish context = () => {
      role = new Role(roleName);
      for (int i = 0; i < numUsers; i++) {
        var user = new User("user" + i, "test", "test", "test", "test");
        role.Users.Add(user);
      }
      roleRepository.Expect(r => r.FindOne(null)).IgnoreArguments().Return(role);
    };

    Because of = () => result = service.GetUsersInRole(roleName);

    It should_return_the_correct_users = () => {
      result.Length.ShouldEqual(numUsers);
      for (int i = 0; i < numUsers; i++) {
        result[i].ShouldEqual(role.Users[i].Username);
      }
    };
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_if_nonadmin_user_is_administrator : specification_for_SharpArchRoleService
  {
    static bool result;
    static string username = "user";

    Establish context = () => {
      var role = new Role(Roles.Administrators);
      roleRepository.Expect(r => r.FindOne(null)).IgnoreArguments().Return(role);
    };

    Because of = () => result = service.IsUserAdministrator(username);

    It should_return_false = () => result.ShouldBeFalse();
  }

  [Subject(typeof(SharpArchRoleService))]
  public class when_the_role_service_is_asked_if_admin_user_is_administrator : specification_for_SharpArchRoleService
  {
    static bool result;
    static string username = "user";

    Establish context = () => {
      var role = new Role(Roles.Administrators);
      role.Users.Add(new User(username, "test", "test", "test", "test"));
      roleRepository.Expect(r => r.FindOne(null)).IgnoreArguments().Return(role);
    };

    Because of = () => result = service.IsUserAdministrator(username);

    It should_return_true = () => result.ShouldBeTrue();
  }

}
