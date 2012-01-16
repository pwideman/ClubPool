using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Models;
using ClubPool.Testing;
using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Web.Controllers;

namespace ClubPool.Tests.Controllers.Users
{
  public abstract class EditTest : UsersControllerTest
  {
    protected User user;
    protected List<Role> roles;
    protected Role adminRole;
    protected Role officerRole;
    protected string password = "pass";

    public override void EstablishContext() {
      base.EstablishContext();
      roles = new List<Role>();
      adminRole = new Role(Roles.Administrators);
      adminRole.SetIdTo(1);
      roles.Add(adminRole);
      officerRole = new Role(Roles.Officers);
      officerRole.SetIdTo(2);
      roles.Add(officerRole);
      user = new User("user", password, "user", "one", "user@user.com");
      user.SetIdTo(1);
      user.SetVersionTo(1);
      repository.Setup(r => r.Get<User>(user.Id)).Returns(user);
      repository.Init<Role>(roles.AsQueryable(), true);
    }

  }

  public abstract class EditGetTest : EditTest
  {
    protected ViewResultHelper<EditViewModel> resultHelper;

    public override void When() {
      resultHelper = new ViewResultHelper<EditViewModel>(controller.Edit(user.Id));
    }
  }
}

namespace ClubPool.Tests.Controllers.Users.when_asked_for_the_edit_view
{
  [TestFixture]
  public class by_an_admin_user : EditGetTest
  {
    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Administrators };
    }

    [Test]
    public void it_should_initialize_the_Username_field() {
      resultHelper.Model.Username.Should().Be(user.Username);
    }

    [Test]
    public void it_should_initialize_the_FirstName_field() {
      resultHelper.Model.FirstName.Should().Be(user.FirstName);
    }

    [Test]
    public void it_should_initialize_the_LastName_field() {
      resultHelper.Model.LastName.Should().Be(user.LastName);
    }

    [Test]
    public void it_should_show_the_status_fields() {
      resultHelper.Model.ShowStatus.Should().BeTrue();
    }

    [Test]
    public void it_should_initialize_the_Approved_field() {
      resultHelper.Model.IsApproved.Should().Be(user.IsApproved);
    }

    [Test]
    public void it_should_initialize_the_Locked_field() {
      resultHelper.Model.IsLocked.Should().Be(user.IsLocked);
    }

    [Test]
    public void it_should_initialize_the_Email_field() {
      resultHelper.Model.Email.Should().Be(user.Email);
    }

    [Test]
    public void it_should_show_the_roles_fields() {
      resultHelper.Model.ShowRoles.Should().BeTrue();
    }

    [Test]
    public void it_should_initialize_the_users_roles() {
      user.Roles.Each(r => resultHelper.Model.Roles.Should().Contain(r.Id));
    }

    [Test]
    public void it_should_initialize_the_available_roles() {
      roles.Each(r => resultHelper.Model.AvailableRoles.Where(ar => ar.Id == r.Id).Any().Should().BeTrue());
    }

    [Test]
    public void it_should_show_the_password() {
      resultHelper.Model.ShowPassword.Should().BeTrue();
    }
  }

  [TestFixture]
  public class for_a_normal_user_by_an_officer : EditGetTest
  {

    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
    }

    [Test]
    public void it_should_initialize_the_Username_field() {
      resultHelper.Model.Username.Should().Be(user.Username);
    }

    [Test]
    public void it_should_initialize_the_FirstName_field() {
      resultHelper.Model.FirstName.Should().Be(user.FirstName);
    }

    [Test]
    public void it_should_initialize_the_LastName_field() {
      resultHelper.Model.LastName.Should().Be(user.LastName);
    }

    [Test]
    public void it_should_show_the_status_fields() {
      resultHelper.Model.ShowStatus.Should().BeTrue();
    }

    [Test]
    public void it_should_initialize_the_Approved_field() {
      resultHelper.Model.IsApproved.Should().Be(user.IsApproved);
    }

    [Test]
    public void it_should_initialize_the_Locked_field() {
      resultHelper.Model.IsLocked.Should().Be(user.IsLocked);
    }

    [Test]
    public void it_should_initialize_the_Email_field() {
      resultHelper.Model.Email.Should().Be(user.Email);
    }

    [Test]
    public void it_should_not_show_the_roles_fields() {
      resultHelper.Model.ShowRoles.Should().BeFalse();
    }

    [Test]
    public void it_should_not_show_the_password() {
      resultHelper.Model.ShowPassword.Should().BeFalse();
    }
  }

  [TestFixture]
  public class for_an_officer_by_a_different_officer : EditGetTest
  {

    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      user.AddRole(officerRole);
    }

    [Test]
    public void it_should_initialize_the_Username_field() {
      resultHelper.Model.Username.Should().Be(user.Username);
    }

    [Test]
    public void it_should_initialize_the_FirstName_field() {
      resultHelper.Model.FirstName.Should().Be(user.FirstName);
    }

    [Test]
    public void it_should_initialize_the_LastName_field() {
      resultHelper.Model.LastName.Should().Be(user.LastName);
    }

    [Test]
    public void it_should_show_the_status_fields() {
      resultHelper.Model.ShowStatus.Should().BeTrue();
    }

    [Test]
    public void it_should_initialize_the_Approved_field() {
      resultHelper.Model.IsApproved.Should().Be(user.IsApproved);
    }

    [Test]
    public void it_should_initialize_the_Locked_field() {
      resultHelper.Model.IsLocked.Should().Be(user.IsLocked);
    }

    [Test]
    public void it_should_initialize_the_Email_field() {
      resultHelper.Model.Email.Should().Be(user.Email);
    }

    [Test]
    public void it_should_not_show_the_roles_fields() {
      resultHelper.Model.ShowRoles.Should().BeFalse();
    }

    [Test]
    public void it_should_not_show_the_password() {
      resultHelper.Model.ShowPassword.Should().BeFalse();
    }
  }

  [TestFixture]
  public class for_an_officer_by_himself : EditGetTest
  {
    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      authenticationService.MockPrincipal.User = user;
      user.AddRole(officerRole);
    }

    [Test]
    public void it_should_initialize_the_Username_field() {
      resultHelper.Model.Username.Should().Be(user.Username);
    }

    [Test]
    public void it_should_initialize_the_FirstName_field() {
      resultHelper.Model.FirstName.Should().Be(user.FirstName);
    }

    [Test]
    public void it_should_initialize_the_LastName_field() {
      resultHelper.Model.LastName.Should().Be(user.LastName);
    }

    [Test]
    public void it_should_initialize_the_Email_field() {
      resultHelper.Model.Email.Should().Be(user.Email);
    }

    [Test]
    public void it_should_not_show_the_status_fields() {
      resultHelper.Model.ShowStatus.Should().BeFalse();
    }

    [Test]
    public void it_should_not_show_the_roles_fields() {
      resultHelper.Model.ShowRoles.Should().BeFalse();
    }

    [Test]
    public void it_should_show_the_password() {
      resultHelper.Model.ShowPassword.Should().BeTrue();
    }
  }

  [TestFixture]
  public class for_normal_user_by_himself : EditGetTest
  {
    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[0];
      authenticationService.MockPrincipal.User = user;
    }

    [Test]
    public void it_should_initialize_the_Username_field() {
      resultHelper.Model.Username.Should().Be(user.Username);
    }

    [Test]
    public void it_should_initialize_the_FirstName_field() {
      resultHelper.Model.FirstName.Should().Be(user.FirstName);
    }

    [Test]
    public void it_should_initialize_the_LastName_field() {
      resultHelper.Model.LastName.Should().Be(user.LastName);
    }

    [Test]
    public void it_should_initialize_the_Email_field() {
      resultHelper.Model.Email.Should().Be(user.Email);
    }

    [Test]
    public void it_should_not_show_the_status_fields() {
      resultHelper.Model.ShowStatus.Should().BeFalse();
    }

    [Test]
    public void it_should_not_show_the_roles_fields() {
      resultHelper.Model.ShowRoles.Should().BeFalse();
    }

    [Test]
    public void it_should_show_the_password() {
      resultHelper.Model.ShowPassword.Should().BeTrue();
    }
  }

  [TestFixture]
  public class for_an_admin_user_by_an_officer : EditGetTest
  {
    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[1] { Roles.Officers };
      user.AddRole(adminRole);
    }

    [Test]
    public void it_should_initialize_the_Username_field() {
      resultHelper.Model.Username.Should().Be(user.Username);
    }

    [Test]
    public void it_should_initialize_the_FirstName_field() {
      resultHelper.Model.FirstName.Should().Be(user.FirstName);
    }

    [Test]
    public void it_should_initialize_the_LastName_field() {
      resultHelper.Model.LastName.Should().Be(user.LastName);
    }

    [Test]
    public void it_should_initialize_the_Email_field() {
      resultHelper.Model.Email.Should().Be(user.Email);
    }

    [Test]
    public void it_should_not_show_the_status_fields() {
      resultHelper.Model.ShowStatus.Should().BeFalse();
    }

    [Test]
    public void it_should_not_show_the_roles_fields() {
      resultHelper.Model.ShowRoles.Should().BeFalse();
    }

    [Test]
    public void it_should_not_show_the_password() {
      resultHelper.Model.ShowPassword.Should().BeFalse();
    }
  }

  [TestFixture]
  public class for_a_normal_user_by_a_different_normal_user : EditGetTest
  {
    public override void Given() {
      authenticationService.MockPrincipal.Roles = new string[0];
      authenticationService.MockPrincipal.MockIdentity.Name = "test";
    }

    [Test]
    public void it_should_return_the_error_view() {
      resultHelper.Result.ViewName.Should().Be("Error");
    }

    [Test]
    public void it_should_indicate_an_error() {
      controller.TempData.Keys.Should().Contain(GlobalViewDataProperty.PageErrorMessage);
    }
  }

}
