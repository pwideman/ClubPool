using System;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Models;
using ClubPool.Testing;

namespace ClubPool.Tests.Services.Membership
{
  public abstract class UsernameOrEmailInUseTest : ClubPoolMembershipServiceSpecificationContext
  {
    protected User user;
    protected bool result;

    public override void Given() {
      user = new User("test", "test", "test", "test", "test");
      users.Add(user);
    }
  }

  [TestFixture]
  public class when_asked_if_username_is_in_use_and_it_is : UsernameOrEmailInUseTest
  {
    public override void When() {
      result = service.UsernameIsInUse(user.Username);
    }

    [Test]
    public void it_should_return_true() {
      result.Should().BeTrue();
    }
  }

  [TestFixture]
  public class when_asked_if_username_is_in_use_and_it_is_not : UsernameOrEmailInUseTest
  {
    public override void When() {
      result = service.UsernameIsInUse(user.Username + "1");
    }

    [Test]
    public void it_should_return_false() {
      result.Should().BeFalse();
    }
  }

  [TestFixture]
  public class when_asked_if_email_is_in_use_and_it_is : UsernameOrEmailInUseTest
  {
    public override void When() {
      result = service.EmailIsInUse(user.Email);
    }

    [Test]
    public void it_should_return_true() {
      result.Should().BeTrue();
    }
  }

  [TestFixture]
  public class when_asked_if_email_is_in_use_and_it_is_not : UsernameOrEmailInUseTest
  {
    public override void When() {
      result = service.EmailIsInUse(user.Email + "1");
    }

    [Test]
    public void it_should_return_false() {
      result.Should().BeFalse();
    }
  }

}
