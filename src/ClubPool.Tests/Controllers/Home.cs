﻿using System;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Testing;

using ClubPool.Web.Controllers.Home;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Infrastructure.Configuration;

namespace ClubPool.Tests.Controllers.Home
{
  public abstract class HomeControllerTest : SpecificationContext
  {
    protected HomeController controller;
    protected Mock<IAuthenticationService> authenticationService;

    public override void EstablishContext() {
      authenticationService = new Mock<IAuthenticationService>();
      var config = new ClubPoolConfiguration {
        SiteName = "test",
        SmtpHost = "test",
        SystemEmailAddress = "test@test.com",
        SystemEmailPassword = "test",
        UseRescues = false
      };

      controller = new HomeController(authenticationService.Object, config);
    }
  }

  [TestFixture]
  public class when_asked_for_the_default_view_when_user_is_not_logged_in : HomeControllerTest
  {
    private ViewResultHelper resultHelper;

    public override void  Given() {
      authenticationService.Setup(svc => svc.IsLoggedIn()).Returns(false);
    }

    public override void When() {
      resultHelper = new ViewResultHelper(controller.Index());
    }

    [Test]
    public void it_should_add_sidebar_gadget_collection_to_the_view_data() {
      resultHelper.SidebarGadgets.Should().NotBeNull();
    }

    [Test]
    public void it_should_add_login_gadget_to_sidebar_gadget_collection() {
      resultHelper.SidebarGadgets.ContainsKey("Login").Should().BeTrue();
    }
  }
}
