using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Login;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Membership;

namespace ClubPool.Tests.Controllers.Login
{
  public class LoginControllerTest : SpecificationContext
  {
    protected LoginController controller;
    protected Mock<IRepository> repository;
    protected MockAuthenticationService authenticationService;
    protected Mock<IMembershipService> membershipService;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      authenticationService = AuthHelper.CreateMockAuthenticationService();
      membershipService = new Mock<IMembershipService>();
      controller = new LoginController(authenticationService, membershipService.Object, repository.Object);
    }
  }
}

namespace ClubPool.Tests.Controllers.Login.when_asked_for_the_login_view
{
  [TestFixture]
  public class and_the_user_is_not_logged_in : LoginControllerTest
  {
    private ViewResultHelper<LoginViewModel> resultHelper;
    private string returnUrl;

    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = false;
      returnUrl = "some return url";
    }

    public override void When() {
      resultHelper = new ViewResultHelper<LoginViewModel>(controller.Login(returnUrl));
    }

    [Test]
    public void it_should_set_the_return_url() {
      resultHelper.Model.ReturnUrl.Should().Be(returnUrl);
    }

    [Test]
    public void it_should_set_the_password_to_empty() {
      resultHelper.Model.Password.Should().BeNull();
    }

    [Test]
    public void it_should_set_the_username_to_empty() {
      resultHelper.Model.Username.Should().BeNull();
    }

    [Test]
    public void it_should_set_StayLoggedIn_to_false() {
      resultHelper.Model.StayLoggedIn.Should().BeFalse();
    }
  }

  [TestFixture]
  public class and_the_user_is_logged_in : LoginControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Login(string.Empty));
    }

    [Test]
    public void it_should_redirect_to_the_dashboard_view() {
      resultHelper.ShouldRedirectTo("dashboard", "dashboard");
    }
  }
}

namespace ClubPool.Tests.Controllers.Login.when_asked_to_login
{
  [TestFixture]
  public class with_a_return_url_and_authentication_is_successful : LoginControllerTest
  {
    private RedirectResultHelper resultHelper;
    private string username = "TestUser";
    private string password = "TestPassword";
    private bool stayLoggedIn = true;
    private string returnUrl = "some url";
    private LoginViewModel viewModel;

    public override void Given() {
      membershipService.Setup(s => s.ValidateUser(username, password)).Returns(true);
      viewModel = new LoginViewModel {
        Username = username,
        Password = password,
        StayLoggedIn = stayLoggedIn,
        ReturnUrl = returnUrl
      };
    }

    public override void When() {
      resultHelper = new RedirectResultHelper(controller.Login(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_return_url() {
      resultHelper.Result.Url.Should().Be(returnUrl);
    }
  }

  [TestFixture]
  public class without_a_return_url_and_authentication_is_successful : LoginControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;
    private string username = "TestUser";
    private string password = "TestPassword";
    private bool stayLoggedIn = true;
    private LoginViewModel viewModel;

    public override void Given() {
      membershipService.Setup(s => s.ValidateUser(username, password)).Returns(true);
      viewModel = new LoginViewModel {
        Username = username,
        Password = password,
        StayLoggedIn = stayLoggedIn
      };
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Login(viewModel));
    }

    [Test]
    public void it_should_redirect_to_the_dashboard_view() {
      resultHelper.ShouldRedirectTo("dashboard", "dashboard");
    }
  }

  [TestFixture]
  public class and_authentication_is_unsuccessful : LoginControllerTest
  {
    private ViewResultHelper<LoginViewModel> resultHelper;
    private string username = "TestUser";
    private string password = "TestPassword";
    private LoginViewModel viewModel;

    public override void Given() {
      membershipService.Setup(s => s.ValidateUser(username, password)).Returns(false);
      viewModel = new LoginViewModel {
        Username = username,
        Password = password,
      };
    }

    public override void When() {
      resultHelper = new ViewResultHelper<LoginViewModel>(controller.Login(viewModel));
    }

    [Test]
    public void it_should_set_the_username_to_the_value_provided_by_the_user() {
      resultHelper.Model.Username.Should().Be(username);
    }

    [Test]
    public void it_should_set_the_password_to_empty() {
      resultHelper.Model.Password.Should().BeEmpty();
    }

    [Test]
    public void it_should_set_the_page_error_message() {
      resultHelper.Result.TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage).Should().BeTrue();
    }
  }
}

namespace ClubPool.Tests.Controllers.Login.when_asked_for_the_login_status_view
{
  [TestFixture]
  public class and_the_user_is_not_logged_in : LoginControllerTest
  {
    private PartialViewResultHelper<LoginStatusViewModel> resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = false;
    }

    public override void When() {
      resultHelper = new PartialViewResultHelper<LoginStatusViewModel>(controller.LoginStatus());
    }

    [Test]
    public void it_should_set_the_username_to_empty() {
      resultHelper.Model.Username.Should().BeNull();
    }

    [Test]
    public void it_should_indicate_that_the_user_is_not_logged_in() {
      resultHelper.Model.UserIsLoggedIn.Should().BeFalse();
    }
  }

  [TestFixture]
  public class and_the_user_is_logged_in : LoginControllerTest
  {
    private PartialViewResultHelper<LoginStatusViewModel> resultHelper;
    private string username = "TestUser";

    public override void Given() {
      var principal = authenticationService.MockPrincipal;
      principal.MockIdentity.Name = username;
      principal.MockIdentity.IsAuthenticated = true;
    }

    public override void When() {
      resultHelper = new PartialViewResultHelper<LoginStatusViewModel>(controller.LoginStatus());
    }

    [Test]
    public void it_should_set_the_username_to_the_username_of_the_user() {
      resultHelper.Model.Username.Should().Be(username);
    }

    [Test]
    public void it_should_indicate_that_the_user_is_logged_in() {
      resultHelper.Model.UserIsLoggedIn.Should().BeTrue();
    }
  }
}

namespace ClubPool.Tests.Controllers.Login
{
  [TestFixture]
  public class when_asked_to_logout : LoginControllerTest
  {
    private RedirectToRouteResultHelper resultHelper;

    public override void Given() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated = true;
    }

    public override void When() {
      resultHelper = new RedirectToRouteResultHelper(controller.Logout());
    }

    [Test]
    public void it_should_log_the_user_out() {
      authenticationService.MockPrincipal.MockIdentity.IsAuthenticated.Should().BeFalse();
    }

    [Test]
    public void it_should_redirect_to_the_home_view() {
      resultHelper.ShouldRedirectTo("index", "home");
    }
  }

  [TestFixture]
  public class when_asked_for_the_login_sidebar_gadget : LoginControllerTest
  {
    private PartialViewResultHelper<LoginViewModel> resultHelper;

    public override void When() {
      resultHelper = new PartialViewResultHelper<LoginViewModel>(controller.LoginGadget());
    }

    [Test]
    public void it_should_set_the_username_to_empty() {
      resultHelper.Model.Username.Should().BeNullOrEmpty();
    }

    [Test]
    public void it_should_set_the_password_to_empty() {
      resultHelper.Model.Password.Should().BeNullOrEmpty();
    }

    [Test]
    public void it_should_set_the_return_url_to_empty() {
      resultHelper.Model.ReturnUrl.Should().BeNullOrEmpty();
    }
  }

}
