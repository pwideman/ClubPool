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

namespace ClubPool.Tests.Models.UserTests
{
  [TestFixture]
  public class Updating_skill_level
  {
    private User player;
    private Mock<IRepository> repository;
    private List<MatchResult> matchResults;
    private User opponent;
    private Team playerTeam;
    private Team opponentTeam;
    private Meet meet;

    [SetUp]
    public void SetUp() {
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

    private void Act() {
      player.UpdateSkillLevel(GameType.EightBall, repository.Object);
    }

    protected MatchResult CreateResult(int innings, int defensiveShots, int wins, Web.Models.Match match = null) {
      if (null == match) {
        match = new Web.Models.Match(meet, new MatchPlayer(player, playerTeam), new MatchPlayer(opponent, opponentTeam));
      }
      return new MatchResult(player, innings, defensiveShots, wins) { Match = match };
    }

    [Test]
    public void with_no_results_and_no_existing_value() {
      Act();

      player.SkillLevels.Any().Should().BeFalse();
    }

    [Test]
    public void with_no_results_and_an_existing_value() {
      player.AddSkillLevel(new SkillLevel(player, GameType.EightBall, 3));

      Act();

      player.SkillLevels.Any().Should().BeFalse();
    }

    [Test]
    public void with_one_result_and_no_existing_value() {
      matchResults.Add(CreateResult(20, 0, 2));

      Act();

      player.SkillLevels.Count().Should().Be(1, "player should have 1 skill level value");
      player.SkillLevels.First().Value.Should().Be(4, "player's skill level should be 4");
    }

    [Test]
    public void with_one_result_that_has_defensive_shots() {
      matchResults.Add(CreateResult(20, 4, 2));

      Act();

      player.SkillLevels.Count().Should().Be(1, "player should have 1 skill level value");
      player.SkillLevels.First().Value.Should().Be(5, "player's skill level should be 5");
    }

    [Test]
    public void with_two_results() {

    }
  }
}
