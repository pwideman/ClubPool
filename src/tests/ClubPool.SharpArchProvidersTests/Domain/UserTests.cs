using NUnit.Framework;

using SharpArch.Testing.NUnit;

using ClubPool.SharpArchProviders.Domain;

namespace Tests.ClubPool.SharpArchProviders.Domain
{
  [TestFixture]
  public class UserTests
  {
    [Test]
    public void Can_create_user() {
      var name = "name";
      var password = "password";
      var salt = "salt";
      var email = "email@email.com";
      var user = new User { Username = name, Password = password, PasswordSalt = salt, Email = email };

      user.Username.ShouldEqual(name);
      user.Password.ShouldEqual(password);
      user.PasswordSalt.ShouldEqual(salt);
      user.Email.ShouldEqual(email);
    }
  }
}
