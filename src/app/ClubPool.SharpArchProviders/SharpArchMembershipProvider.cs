using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Configuration.Provider;

using SharpArch.Core;
using SharpArch.Core.DomainModel;

using ClubPool.Framework.Extensions;
using ClubPool.Framework.NHibernate;
using ClubPool.SharpArchProviders.Domain;
using ClubPool.SharpArchProviders.Domain.Queries;

namespace ClubPool.SharpArchProviders
{
  public class SharpArchMembershipProvider : MembershipProvider
  {
    protected ILinqRepository<User> userRepository;
    protected MembershipPasswordFormat passwordFormat;
    protected const int SALT_SIZE = 16; // same size as the SqlMembershipProvider

    public SharpArchMembershipProvider() {
      userRepository = SafeServiceLocator<ILinqRepository<User>>.GetService();
    }

    #region MembershipProvider implementation

    public override void Initialize(string name, NameValueCollection config) {
      // Initialize values from Web.config.
      Check.Require(null != config, "config cannot be null");

      if (string.IsNullOrEmpty(name)) {
        name = "ClubPoolMembershipProvider";
      }
      if (string.IsNullOrEmpty(config["description"])) {
        config.Remove("description");
        config.Add("description", "ClubPool Membership Provider");
      }

      base.Initialize(name, config);

      var passwordFormat = config["passwordFormat"];
      if (passwordFormat == null) {
        // default to Hashed
        this.passwordFormat = MembershipPasswordFormat.Hashed;
      }
      else {
        switch (passwordFormat) {
          case "Clear":
            this.passwordFormat = MembershipPasswordFormat.Clear;
            break;
          case "Hashed":
            this.passwordFormat = MembershipPasswordFormat.Hashed;
            break;
          default:
            throw new Exception("Unsupported password format");
        }
      }
    }

    public override string ApplicationName {
      get {
        throw new NotImplementedException();
      }
      set {
        throw new NotImplementedException();
      }
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword) {
      throw new NotImplementedException();
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
      throw new NotImplementedException();
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
      
      // first validate username, password, and email
      if (username.IsNullOrEmptyOrBlank()) {
        status = MembershipCreateStatus.InvalidUserName;
        return null;
      }
      if (password.IsNullOrEmptyOrBlank()) {
        status = MembershipCreateStatus.InvalidPassword;
        return null;
      }
      if (email.IsNullOrEmptyOrBlank()) {
        status = MembershipCreateStatus.InvalidEmail;
        return null;
      }

      // Raise the ValidatingPassword event in case an event handler has been defined.
      var args = new ValidatePasswordEventArgs(username, password, true);
      OnValidatingPassword(args);
      if (args.Cancel) {
        status = MembershipCreateStatus.InvalidPassword;
        return null;
      }

      User user = null;

      using (var tx = userRepository.DbContext.BeginTransaction()) {
        // see if a user by this username already exists
        user = userRepository.FindOne(UserQueries.UserByUsername(username));
        if (null != user) {
          status = MembershipCreateStatus.DuplicateUserName;
          return null;
        }

        if (RequiresUniqueEmail) {
          // see if a user with this email already exists
          user = userRepository.FindOne(UserQueries.UserByEmail(email));
          if (null != user) {
            status = MembershipCreateStatus.DuplicateEmail;
            return null;
          }
        }

        // encode the password and verify the result
        var salt = GenerateSalt(SALT_SIZE);
        password = EncodePassword(password, salt);
        if (password.IsNullOrEmptyOrBlank()) {
          status = MembershipCreateStatus.InvalidPassword;
          return null;
        }

        // everything's ok, create the new user
        user = new User { Username = username, Password = password, PasswordSalt = salt, Email = email };
        user = userRepository.SaveOrUpdate(user);
        status = MembershipCreateStatus.Success;
        return ConvertUserToMembershipUser(user);
      }
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData) {
      
      if (username.IsNullOrEmptyOrBlank()) {
        return false;
      }

      using (var tx = userRepository.DbContext.BeginTransaction()) {
        var user = userRepository.FindOne(UserQueries.UserByUsername(username));
        if (null == user) {
          return false;
        }
        else {
          userRepository.Delete(user);
          // TODO: what to do about deleteAllRelatedData?
          return true;
        }
      }
    }

    public override bool EnablePasswordReset {
      get { return true; }
    }

    public override bool EnablePasswordRetrieval {
      get { return false; }
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
      var users = userRepository.GetAll().WithEmailContaining(emailToMatch).Page(pageIndex, pageSize).ToList();
      totalRecords = users.Count;
      return ConvertUsersToMembershipUsers(users);
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
      var users = userRepository.GetAll().WithUsernameContaining(usernameToMatch).Page(pageIndex, pageSize).ToList();
      totalRecords = users.Count;
      return ConvertUsersToMembershipUsers(users);
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
      var users = userRepository.GetAll().Page(pageIndex, pageSize).ToList();
      totalRecords = users.Count;
      return ConvertUsersToMembershipUsers(users);
    }

    public override int GetNumberOfUsersOnline() {
      throw new NotImplementedException();
    }

    public override string GetPassword(string username, string answer) {
      throw new NotImplementedException("Membership PasswordRetrieval not supported");
    }

    public override MembershipUser GetUser(string username, bool userIsOnline) {
      Check.Require(!username.IsNullOrEmptyOrBlank(), "username cannot be null or empty");
      var user = userRepository.FindOne(UserQueries.UserByUsername(username));
      return null != user ? ConvertUserToMembershipUser(user) : null;
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
      Check.Require(null != providerUserKey && providerUserKey is int, "providerUserKey must be a non null int");
      var user = userRepository.Get((int)providerUserKey);
      return null != user ? ConvertUserToMembershipUser(user) : null;
    }

    public override string GetUserNameByEmail(string email) {
      var user = userRepository.FindOne(UserQueries.UserByEmail(email));
      return (null == user ? null : user.Username);
    }

    public override int MaxInvalidPasswordAttempts {
      get { return 0; }
    }

    public override int MinRequiredNonAlphanumericCharacters {
      get { return 0; }
    }

    public override int MinRequiredPasswordLength {
      get { return 6; }
    }

    public override int PasswordAttemptWindow {
      get { return 0; }
    }

    public override MembershipPasswordFormat PasswordFormat { get { return this.passwordFormat; } }

    public override string PasswordStrengthRegularExpression {
      get { return string.Empty; }
    }

    public override bool RequiresQuestionAndAnswer {
      get { return false; }
    }

    public override bool RequiresUniqueEmail {
      get { return true; }
    }

    public override string ResetPassword(string username, string answer) {
      Check.Require(!username.IsNullOrEmptyOrBlank(), "username cannot be null or empty");

      if (!EnablePasswordReset) {
        throw new MembershipPasswordException("Password reset not enabled");
      }

      using (var tx = userRepository.DbContext.BeginTransaction()) {
        var user = userRepository.FindOne(UserQueries.UserByUsername(username));
        if (null == user) {
          throw new ProviderException("Invalid username");
        }

        var newPassword = Membership.GeneratePassword(MinRequiredPasswordLength, MinRequiredNonAlphanumericCharacters);

        // Raise the ValidatingPassword event in case an event handler has been defined.
        ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);
        OnValidatingPassword(args);
        if (args.Cancel) {
          throw args.FailureInformation ?? new MembershipPasswordException("Reset cancelled");
        }

        user.Password = EncodePassword(newPassword, user.PasswordSalt);
        userRepository.SaveOrUpdate(user);
        return newPassword;
      }
    }

    public override bool UnlockUser(string userName) {
      throw new NotImplementedException();
    }

    public override void UpdateUser(MembershipUser user) {
      Check.Require(null != user, "user cannot be null");

      using (var tx = userRepository.DbContext.BeginTransaction()) {
        // TODO: Use Id instead? Need to find out what the norm is for which properties are
        // allowed to be updated. Username? etc.
        var userEntity = userRepository.FindOne(UserQueries.UserByUsername(user.UserName));
        if (null == userEntity) {
          throw new ProviderException("User not found");
        }
        userEntity.Email = user.Email;
        userRepository.SaveOrUpdate(userEntity);
      }
    }

    public override bool ValidateUser(string username, string password) {
      User user = userRepository.FindOne(UserQueries.UserByUsername(username));
      if (null != user) {
        return VerifyPassword(user.Password, password, user.PasswordSalt);
      }
      else {
        return false;
      }
    }

    #endregion MembershipProvider/IMembershipService implementation

    #region Support Methods

    protected bool VerifyPassword(string password, string passwordToVerify, string salt) {
      switch(PasswordFormat) {
        case MembershipPasswordFormat.Hashed:
          passwordToVerify = EncodePassword(passwordToVerify, salt);
          break;
      }
      return password.Equals(passwordToVerify);
    }

    protected MembershipUserCollection ConvertUsersToMembershipUsers(IList<User> users) {
      var membershipUsers = new MembershipUserCollection();
      foreach (var user in users) {
        var membershipUser = ConvertUserToMembershipUser(user);
        membershipUsers.Add(membershipUser);
      }
      return membershipUsers;
    }

    protected MembershipUser ConvertUserToMembershipUser(User user) {
      var membershipUser = new MembershipUser(Name, user.Username, user.Id, user.Email, null,
        null, true, false, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
      return membershipUser;
    }

    protected string EncodePassword(string password, string salt) {
      switch (PasswordFormat) {
        case MembershipPasswordFormat.Clear:
          return password;
        case MembershipPasswordFormat.Hashed:
          Check.Require(!salt.IsNullOrEmptyOrBlank(), "Hashed passwords require a salt value");
          var hash = new HMACSHA1();
          hash.Key = Convert.FromBase64String(salt);
          return Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
        default:
          throw new ArgumentException("Unsupported password format");
      }
    }

    protected string GenerateSalt(int size) {
      byte[] buf = new byte[size];
      new RNGCryptoServiceProvider().GetBytes(buf);
      return Convert.ToBase64String(buf);
    }

    #endregion Support Methods

  }
}
