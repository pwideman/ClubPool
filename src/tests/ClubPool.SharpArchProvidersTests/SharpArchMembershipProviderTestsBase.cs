using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Diagnostics;
using System.Configuration.Provider;

using Microsoft.Practices.ServiceLocation;
using Rhino.Mocks;
using NUnit.Framework;

using SharpArch.Core;
using SharpArch.Testing.NHibernate;
using SharpArch.Testing.NUnit;
using SharpArch.Testing.NUnit.NHibernate;
using SharpArch.Data.NHibernate;

using ClubPool.Framework.NHibernate;
using ClubPool.SharpArchProviders;
using ClubPool.SharpArchProviders.Domain;
using ClubPool.SharpArchProviders.Domain.Queries;

using Tests.ClubPool.SharpArchProviders.TestDoubles;

namespace Tests.ClubPool.SharpArchProviders
{
  public abstract class SharpArchMembershipProviderTestsBase : ProviderTestsBase
  {
    protected TestSharpArchMembershipProvider provider;

    protected abstract TestSharpArchMembershipProvider GetProvider();

    protected override void InitializeProvider() {
      provider = GetProvider();
    }

    protected override void LoadTestData() {
      for (int i = 0; i < 5; i++) {
        var salt = provider.GenerateSalt(provider.SALT_SIZE);
        var username = "user" + i.ToString();
        var user = new User(username, provider.EncodePassword(username, salt), username + "@email.com");
        user.PasswordSalt = salt;
        user.IsApproved = true;
        provider.UserRepository.SaveOrUpdate(user);
      }
    }

    #region Initialize Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void Initialize_throws_with_null_config() {
      provider.Initialize("name", null);
    }

    #endregion Initialize Tests

    #region CreateUser Tests

    [Test]
    public void CreateUser_returns_correct_status_for_null_username() {
      MembershipCreateStatus status;
      
      var user = provider.CreateUser(null, "password", "email", null, null, true, 1, out status);

      user.ShouldBeNull();
      status.ShouldEqual(MembershipCreateStatus.InvalidUserName);
    }

    [Test]
    public void CreateUser_returns_correct_status_for_null_password() {
      MembershipCreateStatus status;

      var user = provider.CreateUser("username", null, "email", null, null, true, 1, out status);

      user.ShouldBeNull();
      status.ShouldEqual(MembershipCreateStatus.InvalidPassword);
    }

    [Test]
    public void CreateUser_returns_correct_status_for_null_email() {
      MembershipCreateStatus status;

      var user = provider.CreateUser("username", "password", null, null, null, true, 1, out status);

      user.ShouldBeNull();
      status.ShouldEqual(MembershipCreateStatus.InvalidEmail);
    }

    [Test]
    public void CreateUser_returns_correct_status_for_duplicate_username() {
      MembershipCreateStatus status;
      var username = provider.UserRepository.GetAll().First().Username;

      var user = provider.CreateUser(username, "password", "email", null, null, true, 1, out status);

      user.ShouldBeNull();
      status.ShouldEqual(MembershipCreateStatus.DuplicateUserName);
    }

    [Test]
    public void CreateUser_returns_correct_status_for_duplicate_email() {
      MembershipCreateStatus status;
      var email = provider.UserRepository.GetAll().First().Email;

      var user = provider.CreateUser("username", "password", email, null, null, true, 1, out status);

      user.ShouldBeNull();
      status.ShouldEqual(MembershipCreateStatus.DuplicateEmail);
    }

    [Test]
    public void CreateUser_creates_new_user_properly() {
      MembershipCreateStatus status;
      var user = new User("username", "password", "newemail@email.com");
      user.IsApproved = true;

      var membershipUser = provider.CreateUser(user.Username, user.Password, user.Email, null, null, user.IsApproved, 1, out status);

      status.ShouldEqual(MembershipCreateStatus.Success);

      // verify the returned MembershipUser
      membershipUser.ShouldNotBeNull();
      membershipUser.UserName.ShouldEqual(user.Username);
      membershipUser.Email.ShouldEqual(user.Email);
      membershipUser.IsApproved.ShouldEqual(user.IsApproved);
    }

    [Test]
    public void CreateUser_adds_new_user_to_repository() {
      MembershipCreateStatus status;
      var user = new User("username", "password", "newemail@email.com");
      user.IsApproved = true;

      var membershipUser = provider.CreateUser(user.Username, user.Password, user.Email, null, null, user.IsApproved, 1, out status);

      FlushAndClearSession();
      membershipUser.ShouldNotBeNull();
      status.ShouldEqual(MembershipCreateStatus.Success);
      var repoUser = provider.UserRepository.FindOne(UserQueries.UserByUsername(user.Username));
      repoUser.ShouldNotBeNull();
      repoUser.Email.ShouldEqual(user.Email);
      repoUser.Password.ShouldEqual(provider.EncodePassword(user.Password, repoUser.PasswordSalt));
      repoUser.IsApproved.ShouldEqual(user.IsApproved);
    }

    #endregion CreateUser Tests

    #region DeleteUser Tests

    [Test]
    public void DeleteUser_returns_false_for_null_username() {
      var deleted = provider.DeleteUser(null, true);

      deleted.ShouldBeFalse();
    }

    [Test]
    public void DeleteUser_returns_false_for_invalid_username() {
      var deleted = provider.DeleteUser("junk", true);

      deleted.ShouldBeFalse();
    }

    [Test]
    public void DeleteUser_returns_true_for_valid_username() {
      var user = provider.UserRepository.GetAll().First();

      var deleted = provider.DeleteUser(user.Username, true);

      deleted.ShouldBeTrue();
    }

    [Test]
    public void DeleteUser_removes_user_from_repository() {
      var user = provider.UserRepository.GetAll().First();

      provider.DeleteUser(user.Username, true);

      FlushAndClearSession();
      provider.UserRepository.FindOne(UserQueries.UserByUsername(user.Username)).ShouldBeNull();
    }

    #endregion DeleteUser Tests

    #region FindUsersByEmail Tests

    [Test]
    public void FindUsersByEmail_returns_zero_totalRecords_for_invalid_email() {
      var email = "somejunk";
      int totalRecords;

      var users = provider.FindUsersByEmail(email, 0, 1, out totalRecords);

      totalRecords.ShouldEqual(0);
      users.ShouldBeEmpty();
    }

    [Test]
    public void FindUsersByEmail_returns_correct_totalRecords_and_collection() {
      var search = "searchstring";
      var user = MockUserRepositoryFactory.CreateTransientUser("testuser");
      user.Email = search + "1@email.com";
      provider.UserRepository.SaveOrUpdate(user);
      user = MockUserRepositoryFactory.CreateTransientUser("testuser2");
      user.Email = search + "2@email.com";
      provider.UserRepository.SaveOrUpdate(user);
      FlushSessionAndEvict(null);

      int total;
      var users = provider.FindUsersByEmail(search, 0, 2, out total);

      total.ShouldEqual(2);
      users.Count.ShouldEqual(2);
      foreach(MembershipUser memUser in users) {
        memUser.Email.ShouldContain(search);
      }
    }

    [Test]
    public void FindUsersByEmail_honors_pageSize() {
      int total;
      var users = provider.FindUsersByEmail("email", 0, 3, out total);

      total.ShouldEqual(3);
      users.Count.ShouldEqual(3);
    }

    [Test]
    public void FindUsersByEmail_honors_pageIndex() {
      int total;
      var pageIndex = 1;

      var users = provider.FindUsersByEmail("email", pageIndex, 2, out total);

      total.ShouldEqual(2);
      users.Count.ShouldEqual(2);
      // verify that it got the second page of users
      for (int i = 2; i < 4; i++) {
        var user = users["user" + i.ToString()];
        user.ShouldNotBeNull();
      }
    }

    [Test]
    public void FindUsersByEmail_truncates_last_page() {
      int total;
      var numUsers = provider.UserRepository.GetAll().Count();
      var pageSize = numUsers / 2 + 1;
      var users = provider.FindUsersByEmail("email", 1, pageSize, out total);

      total.ShouldEqual(numUsers - pageSize);
      users.Count.ShouldEqual(total);
    }
      
    #endregion

    #region FindUsersByName Tests

    [Test]
    public void FindUsersByName_returns_zero_totalRecords_for_invalid_name() {
      var Name = "somejunk";
      int totalRecords;

      var users = provider.FindUsersByName(Name, 0, 1, out totalRecords);

      totalRecords.ShouldEqual(0);
      users.ShouldBeEmpty();
    }

    [Test]
    public void FindUsersByName_returns_correct_totalRecords_and_collection() {
      var search = "searchstring";
      var user = new User(search + "1", "pass", "email");
      provider.UserRepository.SaveOrUpdate(user);
      user = new User(search + "2", "pass", "email");
      provider.UserRepository.SaveOrUpdate(user);
      FlushSessionAndEvict(null);

      int total;
      var users = provider.FindUsersByName(search, 0, 2, out total);

      total.ShouldEqual(2);
      users.Count.ShouldEqual(2);
      foreach (MembershipUser memUser in users) {
        memUser.UserName.ShouldContain(search);
      }
    }

    [Test]
    public void FindUsersByName_honors_pageSize() {
      int total;
      var users = provider.FindUsersByName("user", 0, 3, out total);

      total.ShouldEqual(3);
      users.Count.ShouldEqual(3);
    }

    [Test]
    public void FindUsersByName_honors_pageIndex() {
      int total;
      var pageIndex = 1;

      var users = provider.FindUsersByName("user", pageIndex, 2, out total);

      total.ShouldEqual(2);
      users.Count.ShouldEqual(2);
      // verify that it got the second page of users
      for (int i = 2; i < 4; i++) {
        var user = users["user" + i.ToString()];
        user.ShouldNotBeNull();
      }
    }

    [Test]
    public void FindUsersByName_truncates_last_page() {
      int total;
      var numUsers = provider.UserRepository.GetAll().Count();
      var pageSize = numUsers / 2 + 1;
      var users = provider.FindUsersByName("user", 1, pageSize, out total);

      total.ShouldEqual(numUsers - pageSize);
      users.Count.ShouldEqual(total);
    }

    #endregion FindUsersByName Tests

    #region GetAllUsers Tests

    [Test]
    public void GetAllUsers_returns_all_users() {
      int total;
      var numUsers = provider.UserRepository.GetAll().Count();
      var users = provider.GetAllUsers(0, numUsers + 1, out total);

      total.ShouldEqual(numUsers);
      users.Count.ShouldEqual(total);
    }

    [Test]
    public void GetAllUsers_honors_pageSize() {
      int total;
      var pageSize = 3;

      var users = provider.GetAllUsers(0, pageSize, out total);

      total.ShouldEqual(pageSize);
      users.Count.ShouldEqual(pageSize);
    }

    [Test]
    public void GetAllUsers_honors_pageIndex() {
      int total;
      var pageIndex = 1;

      var users = provider.GetAllUsers(pageIndex, 2, out total);

      total.ShouldEqual(2);
      users.Count.ShouldEqual(2);
      // verify that it got the second page of users
      for (int i = 2; i < 4; i++) {
        var user = users["user" + i.ToString()];
        user.ShouldNotBeNull();
      }
    }

    #endregion GetAllUsers Tests

    #region GetUser Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void GetUser_throws_on_null_username() {
      var user = provider.GetUser(null, true);
    }

    [Test]
    public void GetUser_returns_null_for_invalid_username() {
      var user = provider.GetUser("junk", true);

      user.ShouldBeNull();
    }

    [Test]
    public void GetUser_returns_correct_user_for_username() {
      var user = provider.UserRepository.GetAll().First();
      var membershipUser = provider.GetUser(user.Username, true);

      membershipUser.ShouldNotBeNull();
      membershipUser.UserName.ShouldEqual(user.Username);
    }

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void GetUser_throws_on_invalid_providerUserKey_type() {
      var user = provider.GetUser(true, true);
    }

    [Test]
    public void GetUser_returns_null_for_invalid_providerUserKey() {
      var user = provider.GetUser(33, true);

      user.ShouldBeNull();
    }

    [Test]
    public void GetUser_returns_correct_user_for_providerUserKey() {
      var user = provider.UserRepository.GetAll().First();
      var membershipUser = provider.GetUser(user.Id, true);

      membershipUser.ShouldNotBeNull();
      membershipUser.UserName.ShouldEqual(user.Username);
    }


    #endregion GetUser Tests

    #region GetUserNameByEmail Tests

    [Test]
    public void GetUserNameByEmail_returns_null_for_invalid_email() {
      var user = provider.GetUserNameByEmail("junk");

      user.ShouldBeNull();
    }

    [Test]
    public void GetUserNameByEmail_returns_correct_username_for_valid_email() {
      var user = provider.UserRepository.GetAll().First();

      var username = provider.GetUserNameByEmail(user.Email);

      username.ShouldEqual(user.Username);
    }

    #endregion GetUserNameByEmail Tests

    #region ResetPassword Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void ResetPassword_throws_on_invalid_username() {
      provider.ResetPassword(null, null);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void ResetPassword_throws_on_unknown_username() {
      provider.ResetPassword("junk", null);
    }

    [Test]
    public void ResetPassword_can_reset_password() {
      var user = provider.UserRepository.GetAll().First();
      FlushAndClearSession();

      var newPassword = provider.ResetPassword(user.Username, null);
      FlushAndClearSession();

      newPassword.ShouldNotBeNull();
      newPassword.Length.ShouldEqual(provider.MinRequiredPasswordLength);
      var repoUser = provider.UserRepository.Get(user.Id);
      repoUser.Password.ShouldEqual(provider.EncodePassword(newPassword, repoUser.PasswordSalt));
    }

    [Test]
    public void Can_validate_user_after_ResetPassword() {
      var user = provider.UserRepository.GetAll().First();

      var newPassword = provider.ResetPassword(user.Username, null);

      provider.ValidateUser(user.Username, newPassword).ShouldBeTrue();
    }

    #endregion ResetPassword Tests

    #region UpdateUser Tests

    [Test]
    [ExpectedException(typeof(PreconditionException))]
    public void UpdateUser_throws_on_null_user() {
      provider.UpdateUser(null);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void UpdateUser_throws_on_invalid_user() {
      var user = provider.UserRepository.GetAll().First();
      var membershipUser = provider.GetUser(user.Username, false);
      provider.UserRepository.Delete(user);
      FlushSessionAndEvict(user);

      provider.UpdateUser(membershipUser);
    }

    [Test]
    [ExpectedException(typeof(ProviderException))]
    public void UpdateUser_throws_on_duplicate_email() {
      var user = provider.UserRepository.GetAll().First();
      var otherUser = provider.UserRepository.GetAll().Skip(1).First();
      var membershipUser = provider.GetUser(user.Username, false);
      membershipUser.Email = otherUser.Email;

      provider.UpdateUser(membershipUser);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void UpdateUser_throws_on_null_or_empty_email() {
      var user = provider.UserRepository.GetAll().First();
      var membershipUser = provider.GetUser(user.Username, false);
      membershipUser.Email = null;

      provider.UpdateUser(membershipUser);
    }

    [Test]
    public void UpdateUser_can_update_user() {
      var user = provider.UserRepository.GetAll().First();
      var membershipUser = provider.GetUser(user.Username, false);
      membershipUser.Email = "newemail@email.com";

      FlushAndClearSession();

      provider.UpdateUser(membershipUser);
      FlushAndClearSession();

      // verify membership user
      var updatedMembershipUser = provider.GetUser(membershipUser.UserName, false);
      updatedMembershipUser.Email.ShouldEqual(membershipUser.Email);
      // verify entity
      var updatedRepoUser = provider.UserRepository.Get(user.Id);
      updatedRepoUser.Email.ShouldEqual(membershipUser.Email);
    }


    #endregion UpdateUser Tests

    #region ValidateUser Tests

    [Test]
    public void ValidateUser_returns_true_for_valid_username_and_password() {
      // we must reset the password since we don't know what it is
      // and it's hashed
      var password = "password";
      var user = provider.UserRepository.GetAll().First();
      user.Password = provider.EncodePassword(password, user.PasswordSalt);

      provider.ValidateUser(user.Username, password).ShouldBeTrue();
    }

    [Test]
    public void ValidateUser_returns_false_for_invalid_username() {
      provider.ValidateUser("junk", "abracadabra").ShouldBeFalse();
    }

    [Test]
    public void ValidateUser_returns_false_for_invalid_password() {
      var user = provider.UserRepository.GetAll().First();
      provider.ValidateUser(user.Username, "bad").ShouldBeFalse();
    }

    [Test]
    public void ValidateUser_returns_false_when_user_is_not_approved() {
      var user = provider.UserRepository.GetAll().First();
      user.IsApproved = false;
      var password = "password";
      user.Password = provider.EncodePassword(password, user.PasswordSalt);
      provider.UserRepository.SaveOrUpdate(user);
      FlushAndClearSession();

      provider.ValidateUser(user.Username, password).ShouldBeFalse();
    }

    #endregion ValidateUser Tests

  }
}
