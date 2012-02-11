using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.UnapprovedUsers;
using ClubPool.Testing;
using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Services.Messaging;
using ClubPool.Web.Infrastructure.Configuration;

namespace ClubPool.Tests.Controllers.UnapprovedUsers
{
  public class UnapprovedUsersControllerTest : SpecificationContext
  {
    protected UnapprovedUsersController controller;
    protected Mock<IRepository> repository;
    protected Mock<IEmailService> emailService;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      emailService = new Mock<IEmailService>();
      var config = new ClubPoolConfiguration {
        SiteName = "test",
        SmtpHost = "test",
        SystemEmailAddress = "test@test.com",
        SystemEmailPassword = "test",
        UseRescues = false
      };
      controller = new UnapprovedUsersController(repository.Object, emailService.Object, config);
    }
  }
}

namespace ClubPool.Tests.Controllers.UnapprovedUsers.when_asked_for_the_unapproved_users_view
{
  [TestFixture]
  public class and_there_are_none : UnapprovedUsersControllerTest
  {
    private ViewResultHelper<UnapprovedUsersViewModel> resultHelper;
    private IList<User> users;

    public override void Given() {
      users = new List<User>();
      repository.Setup(r => r.All<User>()).Returns(users.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<UnapprovedUsersViewModel>(controller.UnapprovedUsers());
    }

    [Test]
    public void it_should_not_return_any_unapproved_users() {
      resultHelper.Model.UnapprovedUsers.Count().Should().Be(0);
    }
  }

  [TestFixture]
  public class and_there_are_some : UnapprovedUsersControllerTest
  {
    private ViewResultHelper<UnapprovedUsersViewModel> resultHelper;
    private IList<User> users;

    public override void Given() {
      users = new List<User>() {
        new User("user1", "user1", "user", "one", "user@user.com"),
        new User("user2", "user2", "user", "two", "user2@user.com"),
        new User("user3", "user3", "user", "three", "user3@user.com") { IsApproved = true }
      };
      repository.Setup(r => r.All<User>()).Returns(users.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<UnapprovedUsersViewModel>(controller.UnapprovedUsers());
    }

    [Test]
    public void it_should_return_the_unapproved_users() {
      users.Where(u => !u.IsApproved).Each(u => resultHelper.Model.UnapprovedUsers.Where(uu => uu.Id == u.Id).Any().Should().BeTrue());
    }
  }
}

namespace ClubPool.Tests.Controllers.UnapprovedUsers
{
  [TestFixture]
  public class when_asked_to_approve_users : UnapprovedUsersControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private IList<User> users;
    private int emailAttempts;

    public override void Given() {
      users = new List<User>() {
        new User("user1", "user1", "user", "one", "user@user.com").SetIdTo(1) as User,
        new User("user2", "user2", "user", "two", "user2@user.com").SetIdTo(2) as User,
        new User("user3", "user3", "user", "three", "user3@user.com").SetIdTo(3) as User
      };
      repository.Setup(r => r.All<User>()).Returns(users.AsQueryable());
      emailService.Setup(s => s.SendSystemEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback(() => {
        emailAttempts++;
      });
      ControllerHelper.CreateMockControllerContext(controller);
      Mock.Get(controller.ControllerContext.HttpContext.Request).Setup(r => r.Url).Returns(new Uri("http://host/users/unapproved"));
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.ApproveUsers(new int[] { 1, 2 }));
    }

    [Test]
    public void it_should_approve_the_selected_users() {
      users[0].IsApproved.Should().BeTrue();
      users[1].IsApproved.Should().BeTrue();
    }

    [Test]
    public void it_should_not_approve_the_unselected_users() {
      users[2].IsApproved.Should().BeFalse();
    }

    [Test]
    public void it_should_redirect_to_the_unapproved_users_view() {
      resultHelper.ShouldRedirectTo("unapprovedusers");
    }

    [Test]
    public void it_should_return_a_notification_message() {
      controller.TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage).Should().BeTrue();
    }

    [Test]
    public void it_should_send_an_email_to_each_user() {
      emailAttempts.Should().Be(2);
    }
  }

}
