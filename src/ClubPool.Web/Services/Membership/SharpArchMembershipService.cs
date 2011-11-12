using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;

using ClubPool.Web.Infrastructure;
using ClubPool.Framework.NHibernate;
using ClubPool.Web.Models;

namespace ClubPool.Web.Services.Membership
{
  public class SharpArchMembershipService : IMembershipService
  {
    protected IRepository repository;
    protected const int SALT_SIZE = 16; // same size as the SqlMembershipProvider
    protected bool allowDuplicateEmail;
    protected bool allowEmptyEmail;

    public SharpArchMembershipService(IRepository repo) : this(repo, false, false) {
    }

    public SharpArchMembershipService(IRepository repo, bool allowDuplicateEmail, bool allowEmptyEmail) {
      Arg.NotNull(repo, "repo");

      repository = repo;
      this.allowDuplicateEmail = allowDuplicateEmail;
      this.allowEmptyEmail = allowEmptyEmail;
    }

    public bool UsernameIsInUse(string username) {
      Arg.NotNull(username, "username");
      return repository.All<User>().Any(u => u.Username.Equals(username));
    }

    public bool EmailIsInUse(string email) {
      Arg.NotNull(email, "email");
      return repository.All<User>().Any(u => u.Email.Equals(email));
    }

    public bool ValidateUser(string username, string password) {
      User user = repository.All<User>().SingleOrDefault(u => u.Username.Equals(username));
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

    public string EncodePassword(string password, string salt) {
      Arg.NotNull(password, "password");
      Arg.NotNull(salt, "Hashed passwords require a salt value");

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

      Arg.NotNull(username, "username");
      Arg.NotNull(password, "password");
      if (!allowEmptyEmail) {
        Arg.NotNull(email, "email");
      }
      Arg.NotNull(firstName, "firstName");
      Arg.NotNull(lastName, "lastName");

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
      user = repository.SaveOrUpdate(user);
      return user;
    }

    protected string GenerateSalt(int size) {
      byte[] buf = new byte[size];
      new RNGCryptoServiceProvider().GetBytes(buf);
      return Convert.ToBase64String(buf);
    }

    public string GeneratePasswordResetToken(User user) {
      return GeneratePasswordResetToken(user, DateTime.Now);
    }
    
    protected string GeneratePasswordResetToken(User user, DateTime date) {
      var ticks = date.Ticks.ToString("0000000000000000000");
      var hash = new HMACSHA512(Convert.FromBase64String(user.PasswordSalt));
      return ticks + Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(ticks + user.Username + user.Password + user.PasswordSalt)));
    }

    public bool ValidatePasswordResetToken(string token, User user) {
      var dt = new DateTime(long.Parse(token.Substring(0, 19)));
      var ts = DateTime.Now - dt;
      if (ts.Days > 0) {
        return false;
      }
      else {
        var validToken = GeneratePasswordResetToken(user, dt);
        return validToken.Equals(token, StringComparison.InvariantCulture);
      }

    }
  }
}
