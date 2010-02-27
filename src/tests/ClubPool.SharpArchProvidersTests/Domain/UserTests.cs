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
      var email = "email@email.com";
      var user = new User(name, password, email);

      user.Username.ShouldEqual(name);
      user.Password.ShouldEqual(password);
      user.Email.ShouldEqual(email);
    }
  }
}
