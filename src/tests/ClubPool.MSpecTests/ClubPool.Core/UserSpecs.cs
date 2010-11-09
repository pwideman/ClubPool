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

    Establish context = () => {
      player = new User("test", "test", "test", "test", "test");
      opponent = new User("opponent", "opponent", "opponent", "opponent", "opponent");
      var season = new Season("test", GameType.EightBall);
      var division = new Division("test", DateTime.Parse("1/1/2011"), season);
      var team1 = new Team("team1", division);
      var team2 = new Team("team2", division);
      team1.AddPlayer(player);
      team2.AddPlayer(opponent);
      meet = new Meet(team1, team2, 1);

      results = new List<MatchResult>();

      matchResultRepository = MockRepository.GenerateStub<IMatchResultRepository>();
      matchResultRepository.Stub(r => r.GetMatchResultsForPlayerAndGameType(player, GameType.EightBall)).Return(results.AsQueryable());
    };

    Because of = () => player.UpdateSkillLevel(GameType.EightBall, matchResultRepository);

    protected static MatchResult CreateResult(int innings, int defensiveShots, int wins, Match match) {
      if (null == match) {
        match = new Match(meet, player, opponent);
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
      results.Add(CreateResult(20, 0, 2, null));
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
      results.Add(CreateResult(20, 0, 2, null));

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
  public class when_asked_to_update_skill_level_with_two_results : specification_for_User
  {
    Establish context = () => {
      results.Add(CreateResult(20, 0, 2, null));
      results.Add(CreateResult(20, 0, 3, null));
    };

    It should_calculate_the_value_correctly = () =>
      player.SkillLevels.First().Value.ShouldEqual(5);
  }

}
