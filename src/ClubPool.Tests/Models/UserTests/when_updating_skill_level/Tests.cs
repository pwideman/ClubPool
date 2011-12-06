using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Testing;

namespace ClubPool.Tests.Models.UserTests.when_updating_skill_level
{
  public abstract class when_updating_skill_level : SpecificationContext
  {
    protected User player;
    protected Mock<IRepository> repository;
    protected List<MatchResult> matchResults;
    protected User opponent;
    protected Team playerTeam;
    protected Team opponentTeam;
    protected Meet meet;

    public override void Given() {
      player = new User("test", "test", "test", "test", "test");
      opponent = new User("opponent", "opponent", "opponent", "opponent", "opponent");
      var season = new Season("test", GameType.EightBall);
      var division = new Division("test", DateTime.Parse("1/1/2011"), season);
      playerTeam = new Team("team1", division);
      opponentTeam = new Team("team2", division);
      playerTeam.AddPlayer(player);
      opponentTeam.AddPlayer(opponent);
      meet = new Meet(playerTeam, opponentTeam, 1);

      repository = new Mock<IRepository>();
      matchResults = new List<MatchResult>();
      repository.Setup(r => r.All<MatchResult>()).Returns(() => matchResults.AsQueryable());
    }

    protected MatchResult CreateResult(int innings, int defensiveShots, int wins, Web.Models.Match match = null) {
      if (null == match) {
        match = new Web.Models.Match(meet, new MatchPlayer(player, playerTeam), new MatchPlayer(opponent, opponentTeam));
      }
      return new MatchResult(player, innings, defensiveShots, wins) { Match = match };
    }

  }

  [TestFixture]
  public class with_no_results_and_no_existing_value : when_updating_skill_level
  {
    public override void When() {
      player.UpdateSkillLevel(GameType.EightBall, repository.Object);
    }

    [Test]
    public void then_it_should_not_add_a_skill_level() {
      player.SkillLevels.Any().Should().BeFalse();
    }
  }

  [TestFixture]
  public class with_no_results_and_an_existing_value : when_updating_skill_level
  {
    public override void When() {
      player.AddSkillLevel(new SkillLevel(player, GameType.EightBall, 3));
      player.UpdateSkillLevel(GameType.EightBall, repository.Object);
    }

    [Test]
    public void then_it_should_remove_the_existing_skill_level() {
      player.SkillLevels.Any().Should().BeFalse();
    }
  }

  [TestFixture]
  public class with_one_result_and_no_existing_value : when_updating_skill_level
  {
    public override void When() {
      matchResults.Add(CreateResult(20, 0, 2));
      player.UpdateSkillLevel(GameType.EightBall, repository.Object);
    }

    [Test]
    public void then_it_should_add_a_skill_level() {
      player.SkillLevels.Count().Should().Be(1);
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(4);
    }
  }

  [TestFixture]
  public class with_one_result_that_has_defensive_shots : when_updating_skill_level
  {
    public override void When() {
      matchResults.Add(CreateResult(20, 4, 2));
      player.UpdateSkillLevel(GameType.EightBall, repository.Object);
    }

    [Test]
    public void then_it_should_add_a_skill_level() {
      player.SkillLevels.Count().Should().Be(1);
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(5);
    }
  }
}
