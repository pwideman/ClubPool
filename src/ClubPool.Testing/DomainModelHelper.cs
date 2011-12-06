using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Testing
{
  public static class DomainModelHelper
  {
    public static List<User> GetUsers(int startingId, int count)
    {
      var users = new List<User>();
      for (var i = 0; i < count; i++)
      {
        var id = startingId + i;
        var user = new User("user" + id.ToString(), "pass", "first" + id.ToString(), "last" + id.ToString(), "user" + id.ToString() + "@email.com");
        user.SetIdTo(id);
        user.SetVersionTo(1);
        users.Add(user);
      }
      return users;
    }

    public static List<User> GetUsers(int count)
    {
      return GetUsers(1, count);
    }

    public static Season CreateTestSeason(IList<User> users,
      IList<Division> divisions,
      IList<Team> teams,
      IList<Meet> meets,
      IList<ClubPool.Web.Models.Match> matches,
      IList<MatchResult> matchResults) {

      // set up admin & officer users
      var userId = 1;
      var user = new User("admin", "admin", "admin", "user", "admin@email.com");
      user.SetIdTo(userId++);
      user.SetVersionTo(1);
      user.AddRole(new Role(Roles.Administrators));
      users.Add(user);
      user = new User("officer", "officer", "officer", "user", "officer@email.com");
      user.SetIdTo(userId++);
      user.SetVersionTo(1);
      user.AddRole(new Role(Roles.Officers));
      users.Add(user);
      // set up the test season
      var season = new Season("test season", GameType.EightBall);
      season.IsActive = true;
      season.SetIdTo(1);
      season.SetVersionTo(1);
      var division = new Division("Test Division", DateTime.Parse("1/1/2011"), season);
      division.SetIdTo(1);
      division.SetVersionTo(1);
      divisions.Add(division);
      season.AddDivision(division);
      for (int j = 1; j < 13; j++) {
        var team = new Team(j.ToString(), division);
        team.SetIdTo(j);
        team.SetVersionTo(1);
        teams.Add(team);
        division.AddTeam(team);
        for (int k = userId; k < userId + 2; k++) {
          user = new User(k.ToString(), "test", k.ToString(), "user", "test");
          user.SetIdTo(k);
          user.SetVersionTo(1);
          team.AddPlayer(user);
          users.Add(user);
        }
        userId += 2;
      }
      var repository = new Mock<IRepository>().Object;
      division.CreateSchedule(repository);
      var meetQuery = from m in division.Meets
                      group m by m.Week into g
                      select new { Week = g.Key, Meets = g };

      foreach (var week in meetQuery) {
        var meetDate = division.StartingDate.AddDays(week.Week);
        int meetId = 1;
        int matchId = 1;
        int matchResultId = 1;
        foreach (var meet in week.Meets) {
          meet.SetIdTo(meetId++);
          meets.Add(meet);
          meet.IsComplete = true;
          foreach (var match in meet.Matches) {
            match.SetIdTo(matchId++);
            match.IsComplete = true;
            var players = match.Players.ToArray();
            var mr = new MatchResult(players[0].Player, 20, 0, 3);
            mr.SetIdTo(matchResultId++);
            mr.SetVersionTo(1);
            match.AddResult(mr);
            matchResults.Add(mr);
            mr = new MatchResult(players[1].Player, 20, 0, 2);
            mr.SetIdTo(matchResultId++);
            mr.SetVersionTo(1);
            match.AddResult(mr);
            matchResults.Add(mr);
            match.Winner = players[0].Player;
            match.DatePlayed = meetDate;
            matches.Add(match);
          }
        }
      }
      return season;
    }

    //public static void SetUpTestRepository<T>(ILinqRepository<T> repository, IEnumerable<T> list) where T : Entity {
    //  repository.Stub(r => r.GetAll()).Return(list.AsQueryable());
    //  repository.Stub(r => r.FindOne(null)).IgnoreArguments().Return(null).WhenCalled(m => {
    //    var criteria = m.Arguments[0] as Expression<Func<T, bool>>;
    //    m.ReturnValue = list.AsQueryable().Where(criteria).SingleOrDefault();
    //  });
    //  foreach (var item in list) {
    //    repository.Stub(r => r.Get(item.Id)).Return(item);
    //  }
    //}


  }
}
