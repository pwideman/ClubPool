using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class Division : VersionedEntity
  {
    private static readonly object scheduleLock = new object();

    public virtual DateTime StartingDate { get; set; }
    public virtual string Name { get; set; }
    public virtual Season Season { get; set; }
    public virtual ICollection<Meet> Meets { get; private set; }
    public virtual ICollection<Team> Teams { get; private set; }

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
      Teams = new HashSet<Team>();
      Meets = new HashSet<Meet>();
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

      if (!Teams.Contains(team)) {
        Teams.Add(team);
        team.Division = this;
      }
    }

    public virtual void RemoveTeam(Team team) {
      Arg.NotNull(team, "team");

      if (Meets.Any()) {
        throw new Exception("This division already has a schedule, teams cannot be added or removed");
      }

      if (Teams.Contains(team)) {
        Teams.Remove(team);
        team.Division = null;
      }
    }

    public virtual void RemoveAllTeams() {
      if (Meets.Any()) {
        throw new Exception("This division already has a schedule, teams cannot be added or removed");
      }

      foreach (var team in Teams) {
        team.Division = null;
      }
      Teams.Clear();
    }

    public virtual bool TeamNameIsInUse(string name) {
      return Teams.Where(t => t.Name.Equals(name)).Any();
    }

    public virtual void ClearSchedule() {
      if (!HasCompletedMatches()) {
        Meets.Clear();
      }
      else {
        throw new Exception("There are already completed matches in this division, the schedule cannot be cleared");
      }
    }

    public virtual void CreateSchedule(IRepository repository, int numberOfByes = 0) {
      Arg.NotNull(repository, "repository");
      Arg.Require(numberOfByes >= 0, "numberOfByes must be >= 0");

      repository.Refresh(this);
      if (Meets.Any()) {
        throw new CreateScheduleException("A schedule for this division already exists");
      }
      lock (scheduleLock) {
        repository.Refresh(this);
        if (Meets.Any()) {
          throw new CreateScheduleException("A schedule for this division already exists");
        }

        var numTeams = Teams.Count;
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
        var scheduleTeams = Teams.OrderBy(t => t.SchedulePriority).ToArray();

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
              if (!Meets.Where(m => m.Teams.Contains(scheduleTeams[j]) && m.Teams.Contains(scheduleTeams[opponent])).Any()) {
                Meet m = new Meet(scheduleTeams[j], scheduleTeams[opponent], i);
                Meets.Add(m);
              }
            }
          }
        }
      }
    }

  }
}
