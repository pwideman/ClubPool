using NUnit.Framework;

using SharpArch.Testing.NUnit;

using ClubPool.SharpArchProviders.Domain;

namespace Tests.ClubPool.SharpArchProviders.Domain
{
  [TestFixture]
  public class RoleTests
  {
    [Test]
    public void Can_create_role() {
      var roleName = "role1";
      var role = new Role(roleName);

      role.Name.ShouldEqual(roleName);
    }
  }
}
