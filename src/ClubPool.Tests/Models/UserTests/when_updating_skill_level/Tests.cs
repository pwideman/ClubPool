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
    protected List<MatchResult> results;
    protected User opponent;
    protected Team playerTeam;
    protected Team opponentTeam;
    protected Meet meet;

    public override void EstablishContext() {
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
      results = new List<MatchResult>();
      repository.Setup(r => r.All<MatchResult>()).Returns(() => results.AsQueryable());
    }

    public override void When() {
      player.UpdateSkillLevel(GameType.EightBall, repository.Object);
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
    public override void Given() {
      // don't need to do anything else
    }

    [Test]
    public void then_it_should_not_add_a_skill_level() {
      player.SkillLevels.Any().Should().BeFalse();
    }
  }

  [TestFixture]
  public class with_no_results_and_an_existing_value : when_updating_skill_level
  {
    public override void Given() {
      player.AddSkillLevel(new SkillLevel(player, GameType.EightBall, 3));
    }

    [Test]
    public void then_it_should_remove_the_existing_skill_level() {
      player.SkillLevels.Any().Should().BeFalse();
    }
  }

  [TestFixture]
  public class with_one_result_and_no_existing_value : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
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
    public override void Given() {
      results.Add(CreateResult(20, 4, 2));
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

  [TestFixture]
  public class with_two_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(5);
    }
  }

  [TestFixture]
  public class with_three_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(6);
    }
  }

  [TestFixture]
  public class with_four_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(6);
    }
  }

  [TestFixture]
  public class with_five_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(20, 0, 5));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(6);
    }
  }

  [TestFixture]
  public class with_six_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(5);
    }
  }

  [TestFixture]
  public class with_seven_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(20, 0, 5));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(6);
    }
  }

  [TestFixture]
  public class with_eight_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(20, 0, 5));
      results.Add(CreateResult(20, 0, 2));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(6);
    }
  }

  [TestFixture]
  public class with_nine_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(20, 0, 5));
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 7));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(6);
    }
  }

  [TestFixture]
  public class with_ten_results : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(20, 0, 5));
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 7));
      results.Add(CreateResult(20, 0, 8));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(7);
    }
  }

  [TestFixture]
  public class with_more_than_ten_results : when_updating_skill_level
  {
    public override void Given() {
      var p1 = new MatchPlayer(player, playerTeam);
      var p2 = new MatchPlayer(opponent, opponentTeam);

      var match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/1/2011") };
      results.Add(CreateResult(20, 0, 7, match));
      results.Add(CreateResult(20, 0, 8, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/2/2011") };
      results.Add(CreateResult(20, 0, 4, match));
      results.Add(CreateResult(20, 0, 1, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/3/2011") };
      results.Add(CreateResult(21, 0, 3, match));
      results.Add(CreateResult(21, 0, 3, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/4/2011") };
      results.Add(CreateResult(20, 0, 5, match));
      results.Add(CreateResult(20, 0, 2, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/5/2011") };
      results.Add(CreateResult(20, 0, 2, match));
      results.Add(CreateResult(20, 0, 3, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/6/2011") };
      results.Add(CreateResult(20, 0, 2, match));
      results.Add(CreateResult(20, 0, 3, match));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(6);
    }
  }

  [TestFixture]
  public class and_order_by_date_includes_more_than_ten_results : when_updating_skill_level
  {
    public override void Given() {
      var p1 = new MatchPlayer(player, playerTeam);
      var p2 = new MatchPlayer(opponent, opponentTeam);

      var match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/1/2011") };
      results.Add(CreateResult(20, 0, 4, match));
      results.Add(CreateResult(20, 0, 1, match));
      results.Add(CreateResult(20, 0, 7, match));
      results.Add(CreateResult(20, 0, 8, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/3/2011") };
      results.Add(CreateResult(21, 0, 3, match));
      results.Add(CreateResult(21, 0, 3, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/4/2011") };
      results.Add(CreateResult(20, 0, 5, match));
      results.Add(CreateResult(20, 0, 2, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/5/2011") };
      results.Add(CreateResult(20, 0, 2, match));
      results.Add(CreateResult(20, 0, 3, match));

      match = new Web.Models.Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/6/2011") };
      results.Add(CreateResult(20, 0, 2, match));
      results.Add(CreateResult(20, 0, 3, match));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(7);
    }
  }

  [TestFixture]
  public class and_match_with_zero_wins_results_in_higher_skill_level : when_updating_skill_level
  {
    public override void Given() {
      results.Add(CreateResult(20, 0, 5));
      results.Add(CreateResult(10, 0, 0));
      results.Add(CreateResult(20, 0, 0));
      results.Add(CreateResult(200, 0, 1));
    }

    [Test]
    public void then_it_should_calculate_the_value_correctly() {
      player.SkillLevels.First().Value.Should().Be(6);
    }
  }

}
