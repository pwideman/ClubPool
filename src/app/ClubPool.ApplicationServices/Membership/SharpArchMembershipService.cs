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
using ClubPool.Core.Contracts;

namespace ClubPool.ApplicationServices.Membership
{
  public class SharpArchMembershipService : IMembershipService
  {
    protected IUserRepository userRepository;
    protected const int SALT_SIZE = 16; // same size as the SqlMembershipProvider
    protected bool allowDuplicateEmail;
    protected bool allowEmptyEmail;

    public SharpArchMembershipService(IUserRepository userRepo) {
      Check.Require(null != userRepo, "The user repository cannot be null");

      userRepository = userRepo;
      allowDuplicateEmail = false;
      allowEmptyEmail = false;
    }

    public SharpArchMembershipService(IUserRepository userRepo, bool allowDuplicateEmail, bool allowEmptyEmail)
      : this(userRepo) {
      this.allowDuplicateEmail = allowDuplicateEmail;
      this.allowEmptyEmail = allowEmptyEmail;
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
      if (null != user && user.IsApproved && !user.IsLocked) {
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
      Check.Require(!string.IsNullOrEmpty(password), "password cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(salt), "Hashed passwords require a salt value");

      byte[] bIn = Encoding.Unicode.GetBytes(password);
      byte[] bSalt = Convert.FromBase64String(salt);
      byte[] bAll = new byte[bSalt.Length + bIn.Length];
      byte[] bRet = null;

      Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
      Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
      HashAlgorithm s = HashAlgorithm.Create("SHA1");
      bRet = s.ComputeHash(bAll);
      return Convert.ToBase64String(bRet);
    }


    public User CreateUser(string username, 
      string password,
      string firstName,
      string lastName,
      string email,
      bool isApproved,
      bool isLocked) {

      Check.Require(!string.IsNullOrEmpty(username), "username cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(password), "password cannot be null or empty");
      if (!allowEmptyEmail) {
        Check.Require(!string.IsNullOrEmpty(email), "email cannot be null or empty");
      }
      Check.Require(!string.IsNullOrEmpty(firstName), "firstName cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(lastName), "lastName cannot be null or empty");

      User user = null;

      // see if a user by this username already exists
      if (UsernameIsInUse(username)) {
        throw new ArgumentException(string.Format("A user with username '{0}' already exists", username));
      }

      // see if a user with this email already exists
      if (!allowDuplicateEmail && EmailIsInUse(email)) {
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
      user.IsLocked = isLocked;
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
