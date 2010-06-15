using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using SharpArch.Core;
using SharpArch.Core.DomainModel;

using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Queries;

namespace ClubPool.ApplicationServices.Membership
{
  public class SharpArchMembershipService : IMembershipService
  {
    protected ILinqRepository<User> userRepository;
    protected const int SALT_SIZE = 16; // same size as the SqlMembershipProvider

    public SharpArchMembershipService(ILinqRepository<User> userRepo) {
      Check.Require(null != userRepo, "The user repository cannot be null");

      userRepository = userRepo;
    }

    public bool UsernameIsInUse(string username) {
      Check.Require(!string.IsNullOrEmpty(username), "username cannot be null or empty");
      return userRepository.GetAll().WithUsername(username).Any();
    }

    public bool EmailIsInUse(string email) {
      Check.Require(!string.IsNullOrEmpty(email), "email cannot be null or empty");
      return userRepository.GetAll().WithEmail(email).Any();
    }

    public bool ValidateUser(string username, string password) {
      User user = userRepository.FindOne(UserQueries.UserByUsername(username));
      if (null != user && user.IsApproved) {
        return VerifyPassword(user.Password, password, user.PasswordSalt);
      }
      else {
        return false;
      }
    }

    protected bool VerifyPassword(string password, string passwordToVerify, string salt) {
      passwordToVerify = EncodePassword(passwordToVerify, salt);
      return password.Equals(passwordToVerify);
    }

    protected string EncodePassword(string password, string salt) {
        Check.Require(!salt.IsNullOrEmptyOrBlank(), "Hashed passwords require a salt value");
        var hash = new HMACSHA1();
        hash.Key = Convert.FromBase64String(salt);
        return Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
    }

    public User CreateUser(string username, string password, string firstName, string lastName, string email, bool isApproved) {
      Check.Require(!string.IsNullOrEmpty(username), "username cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(password), "password cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(email), "email cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(firstName), "firstName cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(lastName), "lastName cannot be null or empty");

      User user = null;

      // see if a user by this username already exists
      if (UsernameIsInUse(username)) {
        throw new ArgumentException(string.Format("A user with username '{0}' already exists", username));
      }

      // see if a user with this email already exists
      if (EmailIsInUse(email)) {
        throw new ArgumentException(string.Format("The email address '{0}' is already in use", email));
      }

      // encode the password and verify the result
      var salt = GenerateSalt(SALT_SIZE);
      var hashedPassword = EncodePassword(password, salt);
      if (string.IsNullOrEmpty(hashedPassword)) {
        throw new ArgumentException(string.Format("Invalid password: '{0}'", password));
      }

      // everything's ok, create the new user
      user = new User(username, hashedPassword, firstName, lastName, email);
      user.PasswordSalt = salt;
      user.IsApproved = isApproved;
      user.FirstName = firstName;
      user.LastName = lastName;
      user = userRepository.SaveOrUpdate(user);
      return user;
    }

    protected string GenerateSalt(int size) {
      byte[] buf = new byte[size];
      new RNGCryptoServiceProvider().GetBytes(buf);
      return Convert.ToBase64String(buf);
    }
  }
}
