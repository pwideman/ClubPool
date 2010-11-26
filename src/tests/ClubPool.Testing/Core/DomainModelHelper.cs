using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using SharpArch.Testing;
using Rhino.Mocks;
using SharpArch.Core.DomainModel;

using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Framework.NHibernate;

namespace ClubPool.Testing.Core
{
  public static class DomainModelHelper
  {
    public static Season CreateTestSeason(IList<User> users,
      IList<Division> divisions,
      IList<Team> teams,
      IList<Meet> meets,
      IList<Match> matches,
      IList<MatchResult> matchResults) {

      // set up the test season
      var season = new Season("test season", GameType.EightBall);
      season.IsActive = true;
      season.SetIdTo(1);
      var userId = 1;
      var division = new Division("Test Division", DateTime.Parse("1/1/2011"), season);
      division.SetIdTo(1);
      divisions.Add(division);
      season.AddDivision(division);
      for (int j = 1; j < 13; j++) {
        var team = new Team(j.ToString(), division);
        teams.Add(team);
        division.AddTeam(team);
        for (int k = userId; k < userId + 2; k++) {
          var user = new User(k.ToString(), "test", k.ToString(), "user", "test");
          user.SetIdTo(k);
          team.AddPlayer(user);
          users.Add(user);
        }
        userId += 2;
      }
      var divisionRepository = MockRepository.GenerateStub<IDivisionRepository>();
      division.CreateSchedule(divisionRepository);
      var meetQuery = from m in division.Meets
                      group m by m.Week into g
                      select new { Week = g.Key, Meets = g };

      foreach (var week in meetQuery) {
        var meetDate = division.StartingDate.AddDays(week.Week);
        foreach (var meet in week.Meets) {
          meets.Add(meet);
          meet.IsComplete = true;
          foreach (var match in meet.Matches) {
            match.IsComplete = true;
            var mr = new MatchResult(match.Player1, 20, 0, 3);
            match.AddResult(mr);
            matchResults.Add(mr);
            mr = new MatchResult(match.Player2, 20, 0, 2);
            match.AddResult(mr);
            matchResults.Add(mr);
            match.Winner = match.Player1;
            match.DatePlayed = meetDate;
            matches.Add(match);
          }
        }
      }
      return season;
    }

    public static void SetUpTestRepository<T>(ILinqRepository<T> repository, IEnumerable<T> list) where T : Entity {
      repository.Stub(r => r.GetAll()).Return(list.AsQueryable());
      repository.Stub(r => r.FindOne(null)).IgnoreArguments().Return(null).WhenCalled(m => {
        var criteria = m.Arguments[0] as Expression<Func<T, bool>>;
        m.ReturnValue = list.AsQueryable().Where(criteria).SingleOrDefault();
      });
      foreach (var item in list) {
        repository.Stub(r => r.Get(item.Id)).Return(item);
      }
    }


  }
}
