using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Teams.ViewModels;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Tests.Controllers.Teams
{
  public class UpdateNameTest : DetailsTest
  {
    protected JsonResultHelper<AjaxUpdateResponseViewModel> resultHelper;
    protected UpdateNameViewModel viewModel;
    protected string newName = "NewName";

    public override void EstablishContext() {
      base.EstablishContext();
      team = teams[0];
      viewModel = new UpdateNameViewModel();
      viewModel.Id = team.Id;
      viewModel.Name = newName;
    }
  }
}

namespace ClubPool.Tests.Controllers.Teams.when_asked_to_update_a_team_name
{
  [TestFixture]
  public class by_an_admin : UpdateNameTest
  {
    public override void Given() {
      authService.MockPrincipal.User = adminUser;
    }

    public override void When() {
      resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));
    }

    [Test]
    public void it_should_update_the_team_name() {
      team.Name.Should().Be(newName);
    }

    [Test]
    public void it_should_return_success() {
      resultHelper.Data.Success.Should().BeTrue();
    }
  }

  [TestFixture]
  public class by_an_officer : UpdateNameTest
  {
    public override void Given() {
      authService.MockPrincipal.User = officerUser;
    }

    public override void When() {
      resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));
    }

    [Test]
    public void it_should_update_the_team_name() {
      team.Name.Should().Be(newName);
    }

    [Test]
    public void it_should_return_success() {
      resultHelper.Data.Success.Should().BeTrue();
    }
  }

  [TestFixture]
  public class by_a_team_member : UpdateNameTest
  {
    public override void Given() {
      authService.MockPrincipal.User = team.Players.First();
    }

    public override void When() {
      resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));
    }

    [Test]
    public void it_should_update_the_team_name() {
      team.Name.Should().Be(newName);
    }

    [Test]
    public void it_should_return_success() {
      resultHelper.Data.Success.Should().BeTrue();
    }
  }

  [TestFixture]
  public class by_a_normal_user_that_is_not_on_the_team : UpdateNameTest
  {
    public override void Given() {
      authService.MockPrincipal.User = teams.Last().Players.First();
    }

    public override void When() {
      resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));
    }

    [Test]
    public void it_should_not_update_the_team_name() {
      team.Name.Should().NotBe(newName);
    }

    [Test]
    public void it_should_return_failure() {
      resultHelper.Data.Success.Should().BeFalse();
    }
  }

  [TestFixture]
  public class with_an_empty_name : UpdateNameTest
  {
    public override void Given() {
      viewModel.Name = "";
      authService.MockPrincipal.User = adminUser;
      controller.ModelState.AddModelError("Name", new Exception("Test"));
    }

    public override void When() {
      resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));
    }

    [Test]
    public void it_should_not_update_the_team_name() {
      team.Name.Should().NotBe(newName);
    }

    [Test]
    public void it_should_return_failure() {
      resultHelper.Data.Success.Should().BeFalse();
    }
  }

  [TestFixture]
  public class with_an_invalid_id : UpdateNameTest
  {
    public override void Given() {
      viewModel.Id = 0;
      authService.MockPrincipal.User = adminUser;
      controller.ModelState.AddModelError("Id", new Exception("Test"));
    }

    public override void When() {
      resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));
    }

    [Test]
    public void it_should_return_failure() {
      resultHelper.Data.Success.Should().BeFalse();
    }
  }

  [TestFixture]
  public class for_a_nonexistent_team : UpdateNameTest
  {
    private HttpNotFoundResultHelper notFoundResultHelper;

    public override void Given() {
      viewModel.Id = 9999;
      authService.MockPrincipal.User = adminUser;
    }

    public override void When() {
      notFoundResultHelper = new HttpNotFoundResultHelper(controller.UpdateName(viewModel));
    }

    [Test]
    public void it_should_return_http_not_found() {
      notFoundResultHelper.Result.Should().NotBeNull();
    }
  }

  [TestFixture]
  public class with_a_name_in_use : UpdateNameTest
  {
    public override void Given() {
      viewModel.Name = teams[2].Name;
      authService.MockPrincipal.User = adminUser;
    }

    public override void When() {
      resultHelper = new JsonResultHelper<AjaxUpdateResponseViewModel>(controller.UpdateName(viewModel));
    }

    [Test]
    public void it_should_not_update_the_team_name() {
      team.Name.Should().NotBe(newName);
    }

    [Test]
    public void it_should_return_failure() {
      resultHelper.Data.Success.Should().BeFalse();
    }
  }

}
