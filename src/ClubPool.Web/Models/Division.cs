using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using ClubPool.Web.Infrastructure;
using ClubPool.Core;

namespace ClubPool.Web.Models
{
  public class Division : VersionedEntity
  {
    private static readonly object scheduleLock = new object();
    private IList<Meet> meets;
    private IList<Team> teams;

    public virtual DateTime StartingDate { get; set; }
    public virtual string Name { get; set; }
    public virtual Season Season { get; set; }
    public virtual IEnumerable<Meet> Meets { get { return meets; } }
    public virtual IEnumerable<Team> Teams { get { return teams; } }

    protected Division() {
      InitMembers();
    }

    public Division(string name, DateTime startingDate, Season season) : this() {
      Arg.NotNull(name, "name");
      Arg.NotNull(startingDate, "startingDate");
      Arg.NotNull(season, "season");

      Name = name;
      StartingDate = startingDate;
      Season = season;
    }

    private void InitMembers() {
      teams = new List<Team>();
      meets = new List<Meet>();
    }

    public virtual bool CanDelete() {
      return !HasCompletedMatches();
    }

    public virtual bool HasCompletedMatches() {
      return Meets.Where(m => m.Matches.Where(match => match.IsComplete).Any()).Any();
    }

    public virtual void AddTeam(Team team) {
      Arg.NotNull(team, "team");

      if (Meets.Any()) {
        throw new Exception("This division already has a schedule, teams cannot be added or removed");
      }

      if (!teams.Contains(team)) {
        teams.Add(team);
        team.Division = this;
      }
    }

    public virtual void RemoveTeam(Team team) {
      Arg.NotNull(team, "team");

      if (Meets.Any()) {
        throw new Exception("This division already has a schedule, teams cannot be added or removed");
      }

      if (teams.Contains(team)) {
        teams.Remove(team);
        team.Division = null;
      }
    }

    public virtual void RemoveAllTeams() {
      if (Meets.Any()) {
        throw new Exception("This division already has a schedule, teams cannot be added or removed");
      }

      foreach (var team in teams) {
        team.Division = null;
      }
      teams.Clear();
    }

    public virtual bool TeamNameIsInUse(string name) {
      return Teams.Where(t => t.Name.Equals(name)).Any();
    }

    public virtual void ClearSchedule() {
      if (!HasCompletedMatches()) {
        meets.Clear();
      }
      else {
        throw new Exception("There are already completed matches in this division, the schedule cannot be cleared");
      }
    }

    public virtual void CreateSchedule(IRepository repository, int numberOfByes = 0) {
      Arg.NotNull(repository, "repository");
      Arg.Require(numberOfByes >= 0, "numberOfByes must be >= 0");

      repository.Refresh(this);
      if (meets.Any()) {
        throw new CreateScheduleException("A schedule for this division already exists");
      }
      lock (scheduleLock) {
        repository.Refresh(this);
        if (meets.Any()) {
          throw new CreateScheduleException("A schedule for this division already exists");
        }

        var numTeams = teams.Count;
        var realNumberOfTeams = numTeams - 1;
        if (numTeams < 2) {
          throw new ArgumentException("division must have 2 or more teams to create a schedule", "division");
        }

        var needBye = (numTeams % 2 != 0) ? true : false;
        if (0 == numberOfByes && needBye) {
          numberOfByes = 1;
        }
        numTeams += numberOfByes;
        var numWeeks = numTeams - 1;
        var opponent = -1;
        var scheduleTeams = teams.OrderBy(t => t.SchedulePriority).ToArray();

        for (int i = 0; i < numWeeks; i++) {
          for (int j = 0; j < numTeams; j++) {
            if (j < (numTeams - 1)) {
              if (i == ((2 * j + 1) % (numTeams - 1))) {
                opponent = numTeams - 1;
              }
              else {
                opponent = ((numTeams - 1) + i - j - 1) % (numTeams - 1);
              }
            }
            else {
              for (int p = 0; p < numTeams; p++) {
                if (i == (2 * p + 1) % (numTeams - 1)) {
                  opponent = p;
                  break;
                }
              }
            }
            if (opponent != j && opponent <= realNumberOfTeams && j <= realNumberOfTeams) {
              if (!meets.Where(m => m.Teams.Contains(scheduleTeams[j]) && m.Teams.Contains(scheduleTeams[opponent])).Any()) {
                Meet m = new Meet(scheduleTeams[j], scheduleTeams[opponent], i);
                meets.Add(m);
              }
            }
          }
        }
      }
    }

  }
}
