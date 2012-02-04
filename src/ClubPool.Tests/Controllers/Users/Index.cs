using System;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;
using Moq;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Users;

namespace ClubPool.Tests.Controllers.Users.when_asked_for_the_default_view
{
  [TestFixture]
  public class with_no_search_query : UsersControllerTest
  {
    private ViewResultHelper<IndexViewModel> resultHelper;
    private int page = 1;
    private int pages = 3;
    private int pageSize = 10;

    public override void Given() {
      var users = new List<User>();
      for (var i = 0; i < pages * pageSize; i++) {
        users.Add(new User("user" + i.ToString(), "pass", "user", i.ToString(), "user" + i.ToString() + "@user.com"));
      }
      repository.Setup(r => r.All<User>()).Returns(users.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index(page, null));
    }

    [Test]
    public void it_should_set_the_number_of_users_to_the_page_size() {
      resultHelper.Model.Items.Count().Should().Be(pageSize);
    }

    [Test]
    public void it_should_set_the_first_user_index() {
      resultHelper.Model.First.Should().Be((page - 1) * pageSize + 1);
    }

    [Test]
    public void it_should_set_the_last_user_index() {
      resultHelper.Model.Last.Should().Be(pageSize * page);
    }

    [Test]
    public void it_should_set_the_current_page_index() {
      resultHelper.Model.CurrentPage.Should().Be(page);
    }

    [Test]
    public void it_should_set_the_total_number_of_users() {
      resultHelper.Model.Total.Should().Be(pageSize * pages);
    }

    [Test]
    public void it_should_set_the_total_pages() {
      resultHelper.Model.TotalPages.Should().Be(pages);
    }
  }

  [TestFixture]
  public class with_search_query : UsersControllerTest
  {
    private ViewResultHelper<IndexViewModel> resultHelper;

    public override void Given() {
      var users = new List<User>();
      users.Add(new User("a", "a", "a", "a", "a"));
      users.Add(new User("b", "b", "b", "b", "b"));
      users.Add(new User("c", "c", "c", "c", "c"));

      repository.Setup(r => r.All<User>()).Returns(users.AsQueryable());
      repository.Setup(r => r.SqlQuery<User>(It.IsAny<string>())).Returns(users.Where(u => u.Username.Contains("a") || u.Username.Contains("c")).AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index(1, "a c"));
    }

    [Test]
    public void it_should_only_return_users_matching_the_query() {
      resultHelper.Model.Total.Should().Be(2);
    }
  }

}
