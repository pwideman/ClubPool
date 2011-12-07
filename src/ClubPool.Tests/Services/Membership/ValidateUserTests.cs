using System;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Models;
using ClubPool.Testing;

namespace ClubPool.Tests.Services.Membership
{
  public abstract class ValidateUserTest : ClubPoolMembershipServiceSpecificationContext
  {
    protected bool result;
    protected string username = "test";
    protected string password = "test";
    protected User user;

    public override void Given() {
      var salt = PasswordHelper.GenerateSalt(16);
      user = new User(username, PasswordHelper.EncodePassword(password, salt), "test", "test", "test");
      user.IsApproved = true;
      user.PasswordSalt = salt;
      users.Add(user);
    }
  }

  [TestFixture]
  public class when_asked_to_validate_an_invalid_username : ValidateUserTest
  {
    public override void When() {
      result = service.ValidateUser("badusername", "badpassword");
    }

    [Test]
    public void it_should_return_false() {
      result.Should().BeFalse();
    }
  }

  [TestFixture]
  public class when_asked_to_validate_an_unapproved_user : ValidateUserTest
  {
    public override void Given() {
      base.Given();
      user.IsApproved = false;
    }

    public override void When() {
      result = service.ValidateUser(username, password);
    }

    [Test]
    public void it_should_return_false() {
      result.Should().BeFalse();
    }
  }

  [TestFixture]
  public class when_asked_to_validate_a_valid_username_and_password : ValidateUserTest
  {
    public override void When() {
      result = service.ValidateUser(username, password);
    }

    [Test]
    public void it_should_return_true() {
      result.Should().BeTrue();
    }
  }
}
