using System;
using System.Collections.Generic;
using System.Linq;

using Rhino.Mocks;
using Machine.Specifications;

using ClubPool.Core;
using ClubPool.Core.Contracts;

namespace ClubPool.MSpecTests.ClubPool.Core
{
  public class specification_for_User
  {
    protected static IMatchResultRepository matchResultRepository;
    protected static User player;
    protected static User opponent;
    protected static Meet meet;
    protected static IList<MatchResult> results;
    protected static Team playerTeam;
    protected static Team opponentTeam;

    Establish context = () => {
      player = new User("test", "test", "test", "test", "test");
      opponent = new User("opponent", "opponent", "opponent", "opponent", "opponent");
      var season = new Season("test", GameType.EightBall);
      var division = new Division("test", DateTime.Parse("1/1/2011"), season);
      playerTeam = new Team("team1", division);
      opponentTeam = new Team("team2", division);
      playerTeam.AddPlayer(player);
      opponentTeam.AddPlayer(opponent);
      meet = new Meet(playerTeam, opponentTeam, 1);

      results = new List<MatchResult>();

      matchResultRepository = MockRepository.GenerateStub<IMatchResultRepository>();
      matchResultRepository.Stub(r => r.GetMatchResultsForPlayerAndGameType(player, GameType.EightBall)).Return(results.AsQueryable());
    };

    Because of = () => player.UpdateSkillLevel(GameType.EightBall, matchResultRepository);

    protected static MatchResult CreateResult(int innings, int defensiveShots, int wins, Match match = null) {
      if (null == match) {
        match = new Match(meet, new MatchPlayer(player, playerTeam), new MatchPlayer(opponent, opponentTeam));
      }
      return new MatchResult(player, innings, defensiveShots, wins) { Match = match };
    }
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_no_results : specification_for_User
  {
    It should_not_add_a_skill_level = () =>
      player.SkillLevels.Any().ShouldBeFalse();
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_no_results_and_an_existing_value : specification_for_User
  {
    Establish context = () => {
      player.AddSkillLevel(new SkillLevel(player, GameType.EightBall, 3));
    };

    It should_remove_the_existing_value = () =>
      player.SkillLevels.Any().ShouldBeFalse();
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_one_result_and_no_existing_value : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
    };

    It should_add_a_new_value = () =>
      player.SkillLevels.Count().ShouldEqual(1);

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(4);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_one_result_and_an_existing_value : specification_for_User
  {
    static SkillLevel sl;

    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));

      sl = new SkillLevel(player, GameType.EightBall, 2);
      player.AddSkillLevel(sl);
    };

    It should_not_add_a_new_value = () => {
      player.SkillLevels.Count().ShouldEqual(1);
      player.SkillLevels.First().ShouldEqual(sl);
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(4);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_one_result_that_has_defensive_shots : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 4, 2));
    };

    It should_add_a_new_value = () =>
      player.SkillLevels.Count().ShouldEqual(1);

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(5);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_two_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(5);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_three_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(6);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_four_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(6);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_five_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(20, 0, 5));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(6);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_six_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(5);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_seven_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(20, 0, 5));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(6);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_eight_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(20, 0, 5));
      results.Add(CreateResult(20, 0, 2));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(6);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_nine_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 3));
      results.Add(CreateResult(20, 0, 4));
      results.Add(CreateResult(20, 0, 1));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(21, 0, 3));
      results.Add(CreateResult(20, 0, 5));
      results.Add(CreateResult(20, 0, 2));
      results.Add(CreateResult(20, 0, 7));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(6);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_ten_results : specification_for_User
  {
    Establish context = () => {
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
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(7);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_with_more_than_ten_results : specification_for_User
  {
    Establish context = () => {
      var p1 = new MatchPlayer(player, playerTeam);
      var p2 = new MatchPlayer(opponent, opponentTeam);

      var match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/1/2011") };
      results.Add(CreateResult(20, 0, 7, match));
      results.Add(CreateResult(20, 0, 8, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/2/2011") };
      results.Add(CreateResult(20, 0, 4, match));
      results.Add(CreateResult(20, 0, 1, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/3/2011") };
      results.Add(CreateResult(21, 0, 3, match));
      results.Add(CreateResult(21, 0, 3, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/4/2011") };
      results.Add(CreateResult(20, 0, 5, match));
      results.Add(CreateResult(20, 0, 2, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/5/2011") };
      results.Add(CreateResult(20, 0, 2, match));
      results.Add(CreateResult(20, 0, 3, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/6/2011") };
      results.Add(CreateResult(20, 0, 2, match));
      results.Add(CreateResult(20, 0, 3, match));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(6);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_and_order_by_date_includes_more_than_10_results : specification_for_User
  {
    Establish context = () => {
      var p1 = new MatchPlayer(player, playerTeam);
      var p2 = new MatchPlayer(opponent, opponentTeam);

      var match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/1/2011") };
      results.Add(CreateResult(20, 0, 4, match));
      results.Add(CreateResult(20, 0, 1, match));
      results.Add(CreateResult(20, 0, 7, match));
      results.Add(CreateResult(20, 0, 8, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/3/2011") };
      results.Add(CreateResult(21, 0, 3, match));
      results.Add(CreateResult(21, 0, 3, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/4/2011") };
      results.Add(CreateResult(20, 0, 5, match));
      results.Add(CreateResult(20, 0, 2, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/5/2011") };
      results.Add(CreateResult(20, 0, 2, match));
      results.Add(CreateResult(20, 0, 3, match));

      match = new Match(meet, p1, p2) { DatePlayed = DateTime.Parse("1/6/2011") };
      results.Add(CreateResult(20, 0, 2, match));
      results.Add(CreateResult(20, 0, 3, match));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(7);
  }

  [Subject(typeof(User))]
  public class when_asked_to_update_skill_level_and_match_with_zero_wins_results_in_higher_skill_level : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 5));
      results.Add(CreateResult(10, 0, 0));
      results.Add(CreateResult(20, 0, 0));
      results.Add(CreateResult(200, 0, 1));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(6);
  }
}
