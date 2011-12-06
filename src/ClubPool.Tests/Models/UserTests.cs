using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using NUnit;
using FluentAssertions;
using Moq;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Models;
using ClubPool.Testing;

namespace ClubPool.Tests.Models
{
  [TestFixture]
  public class UserTests
  {
    private User player;
    private Mock<IRepository> repository;
    private List<MatchResult> matchResults;

    [SetUp]
    public void SetUp()
    {
      player = new User("test", "test", "test", "test", "test");
      repository = new Mock<IRepository>();
      matchResults = new List<MatchResult>();
      repository.Setup(r => r.All<MatchResult>()).Returns(() => matchResults.AsQueryable());
    }

    [Test]
    public void Updating_skill_level_with_no_results_should_not_add_a_skill_level()
    {
      player.UpdateSkillLevel(GameType.EightBall, repository.Object);

      player.SkillLevels.Any().Should().BeFalse();
    }
  }
}
