using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino.Mocks;
using Machine.Specifications;

using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.Web.Services.Membership;
using ClubPool.Testing.Services.Membership;

namespace ClubPool.MSpecTests.ClubPool.ApplicationServices.Membership
{
  public class specification_for_SharpArchMembershipService
  {
    protected static SharpArchMembershipService service;
    protected static IUserRepository userRepository;

    Establish context = () => {
      userRepository = MockRepository.GenerateStub<IUserRepository>();

      service = new SharpArchMembershipService(userRepository);
    };
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_to_validate_an_invalid_username : specification_for_SharpArchMembershipService
  {
    static string username = "test";
    static bool result;

    Establish context = () =>
      userRepository.Expect(r => r.FindOne(UserQueries.UserByUsername(username)))
        .IgnoreArguments()
        .Return(null);

    Because of = () => result = service.ValidateUser(username, "test");

    It should_return_false = () =>
      result.ShouldBeFalse();
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_to_validate_a_user_with_an_invalid_password : specification_for_SharpArchMembershipService
  {
    static string username = "test";
    static string password = "test";
    static string email = "test@test.com";
    static string salt = PasswordHelper.GenerateSalt(16);
    static bool result;
    static User testUser;

    Establish context = () => {
      testUser = new User(username, PasswordHelper.EncodePassword(password, salt), "test", "test", email);
      testUser.PasswordSalt = salt;
      userRepository.Expect(r => r.FindOne(UserQueries.UserByUsername(username)))
        .IgnoreArguments()
        .Return(testUser);
    };

    Because of = () => result = service.ValidateUser(username, "bad");

    It should_return_false = () =>
      result.ShouldBeFalse();
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_to_validate_an_unapproved_user : specification_for_SharpArchMembershipService
  {
    static string username = "test";
    static string password = "test";
    static string email = "test@test.com";
    static string salt = PasswordHelper.GenerateSalt(16);
    static bool result;
    static User testUser;

    Establish context = () => {
      testUser = new User(username, PasswordHelper.EncodePassword(password, salt), "test", "test", email);
      testUser.PasswordSalt = salt;
      testUser.IsApproved = false;
      userRepository.Expect(r => r.FindOne(UserQueries.UserByUsername(username)))
        .IgnoreArguments()
        .Return(testUser);
    };

    Because of = () => result = service.ValidateUser(username, password);

    It should_return_false = () =>
      result.ShouldBeFalse();
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_to_validate_a_valid_username_and_password : specification_for_SharpArchMembershipService
  {
    static string username = "test";
    static string password = "test";
    static string email = "test@test.com";
    static string salt = PasswordHelper.GenerateSalt(16);
    static bool result;
    static User testUser;

    Establish context = () => {
      testUser = new User(username, PasswordHelper.EncodePassword(password, salt), "test", "test", email);
      testUser.PasswordSalt = salt;
      testUser.IsApproved = true;
      userRepository.Expect(r => r.FindOne(UserQueries.UserByUsername(username)))
        .IgnoreArguments()
        .Return(testUser);
    };

    Because of = () => result = service.ValidateUser(username, password);

    It should_return_true = () =>
      result.ShouldBeTrue();
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_to_create_a_user_with_a_duplicate_username : specification_for_SharpArchMembershipService
  {
    static string username = "test";
    static User testUser;
    static Exception theException;

    Establish context = () => {
      testUser = new User(username, "test", "test", "test", "test");
      userRepository.Expect(r => r.FindOne(u => true)).IgnoreArguments().Return(testUser);
    };

    Because of = () => {
      try {
        service.CreateUser(testUser.Username, testUser.Password, testUser.FirstName, 
          testUser.LastName, testUser.Email, false, false);
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

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_to_create_a_user_with_a_duplicate_email : specification_for_SharpArchMembershipService
  {
    static string email = "test";
    static User testUser;
    static Exception theException;

    Establish context = () => {
      testUser = new User("test", "test", "test", "test", email);
      userRepository.Expect(r => r.FindOne(u => true)).IgnoreArguments().Return(testUser);
    };

    Because of = () => {
      try {
        service.CreateUser(testUser.Username, testUser.Password, testUser.FirstName,
          testUser.LastName, testUser.Email, false, false);
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

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_to_create_a_new_user : specification_for_SharpArchMembershipService
  {
    static string username = "test";
    static string password = "test";
    static string firstName = "test";
    static string lastName = "test";
    static string email = "test";
    static User user;

    Establish context = () => {
      userRepository.Expect(r => r.GetAll()).Return(new List<User>().AsQueryable());
      userRepository.Stub(r => r.SaveOrUpdate(Arg<User>.Is.Anything)).Return(null).WhenCalled(m => m.ReturnValue = m.Arguments[0]);
    };

    Because of = () => user = service.CreateUser(username, password, firstName, lastName, email, false, false);

    It should_return_the_new_user = () => {
      user.ShouldNotBeNull();
      user.Username.ShouldEqual(username);
      user.FirstName.ShouldEqual(firstName);
      user.LastName.ShouldEqual(lastName);
      user.Email.ShouldEqual(email);
    };

    It should_save_the_new_user_to_the_repository = () => {
      userRepository.AssertWasCalled(r => r.SaveOrUpdate(user));
    };
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_if_username_is_in_use_and_it_is : specification_for_SharpArchMembershipService
  {
    static string username = "test";
    static bool result;

    Establish context = () => {
      var user = new User(username, "test", "test", "test", "test");
      var users = new List<User>() { user };
      userRepository.Expect(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = service.UsernameIsInUse(username);

    It should_return_true = () => result.ShouldBeTrue();
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_if_username_is_in_use_and_it_is_not : specification_for_SharpArchMembershipService
  {
    static string username = "test";
    static bool result;

    Establish context = () => {
      var user = new User(username, "test", "test", "test", "test");
      var users = new List<User>() { user };
      userRepository.Expect(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = service.UsernameIsInUse("newuser");

    It should_return_true = () => result.ShouldBeFalse();
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_if_email_is_in_use_and_it_is : specification_for_SharpArchMembershipService
  {
    static string email = "test";
    static bool result;

    Establish context = () => {
      var user = new User("test", "test", "test", "test", email);
      var users = new List<User>() { user };
      userRepository.Expect(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = service.EmailIsInUse(email);

    It should_return_true = () => result.ShouldBeTrue();
  }

  [Subject(typeof(SharpArchMembershipService))]
  public class when_the_membership_service_is_asked_if_email_is_in_use_and_it_is_not : specification_for_SharpArchMembershipService
  {
    static string email = "test";
    static bool result;

    Establish context = () => {
      var user = new User("test", "test", "test", "test", email);
      var users = new List<User>() { user };
      userRepository.Expect(r => r.GetAll()).Return(users.AsQueryable());
    };

    Because of = () => result = service.EmailIsInUse("newemail");

    It should_return_true = () => result.ShouldBeFalse();
  }
}
