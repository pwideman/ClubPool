using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using FluentAssertions;

using ClubPool.Testing;
using ClubPool.Web.Controllers.Seasons;
using ClubPool.Web.Models;

namespace ClubPool.Tests.Controllers.Seasons
{
  [TestFixture]
  public class when_asked_for_the_default_view : SeasonsControllerTest
  {
    private int page = 1;
    private int pages = 3;
    private int pageSize = 10;
    private ViewResultHelper<IndexViewModel> resultHelper;

    public override void Given() {
      var seasons = new List<Season>();
      for (var i = 0; i < pages * pageSize; i++) {
        seasons.Add(new Season("season" + i.ToString(), GameType.EightBall));
      }
      repository.Setup(r => r.All<Season>()).Returns(seasons.AsQueryable());
    }

    public override void When() {
      resultHelper = new ViewResultHelper<IndexViewModel>(controller.Index(page));
    }

    [Test]
    public void it_should_set_the_number_of_seasons_to_the_page_size() {
      resultHelper.Model.Items.Count().Should().Be(pageSize);
    }

    [Test]
    public void it_should_set_the_first_season_index() {
      resultHelper.Model.First.Should().Be((page - 1) * pageSize + 1);
    }

    [Test]
    public void it_should_set_the_last_season_index() {
      resultHelper.Model.Last.Should().Be(pageSize * page);
    }

    [Test]
    public void it_should_set_the_current_page_index() {
      resultHelper.Model.CurrentPage.Should().Be(page);
    }

    [Test]
    public void it_should_set_the_total_number_of_seasons() {
      resultHelper.Model.Total.Should().Be(pageSize * pages);
    }

    [Test]
    public void it_should_set_the_total_pages() {
      resultHelper.Model.TotalPages.Should().Be(pages);
    }
  }
}
