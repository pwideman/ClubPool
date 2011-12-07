using System;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Models;
using ClubPool.Testing;

namespace ClubPool.Tests.Services.Membership
{
  public abstract class CreateUserTest : ClubPoolMembershipServiceSpecificationContext
  {
    protected string username = "test";
    protected string email = "test";
    protected Exception theException;

    public override void Given() {
      users.Add(new User(username, "test", "test", "test", email));
    }

    public void WhenCaptureException(Action when) {
      try {
        when();
      }
      catch (Exception e) {
        theException = e;
      }
    }
  }

  [TestFixture]
  public class when_asked_to_create_a_user_with_a_duplicate_username : CreateUserTest
  {
    public override void When() {
      WhenCaptureException(() => service.CreateUser(username, "test1", "test1", "test1", "test1", true, false));
    }

    [Test]
    public void it_should_throw_an_ArgumentException() {
      theException.Should().NotBeNull();
      theException.Should().BeOfType<ArgumentException>();
    }
  }

  [TestFixture]
  public class when_asked_to_create_a_user_with_a_duplicate_email : CreateUserTest
  {
    public override void When() {
      WhenCaptureException(() => service.CreateUser("test1", "test1", "test1", "test1", email, true, false));
    }

    [Test]
    public void it_should_throw_an_ArgumentException() {
      theException.Should().NotBeNull();
      theException.Should().BeOfType<ArgumentException>();
    }
  }

}
