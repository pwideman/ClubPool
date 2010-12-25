using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

using ClubPool.Core.Contracts;

namespace ClubPool.Core
{
  public class Division : Entity, IEntityWithVersion
  {
    private static readonly object scheduleLock = new object();
    protected IList<Meet> meets;
    protected IList<Team> teams;

    public virtual DateTime StartingDate { get; set; }

    [DomainSignature]
    public virtual string Name { get; set; }

    [DomainSignature]
    public virtual Season Season { get; set; }

    public virtual int Version { get; protected set; }

    public virtual IEnumerable<Meet> Meets { get { return meets; } }

    public virtual IEnumerable<Team> Teams { get { return teams; } }

    protected Division() {
      InitMembers();
    }

    public Division(string name, DateTime startingDate, Season season) : this() {
      Check.Require(!string.IsNullOrEmpty(name), "name cannot be null");
      Check.Require(null != startingDate, "startingDate cannot be null");
      Check.Require(null != season, "season cannot be null");

      Name = name;
      StartingDate = startingDate;
      Season = season;
    }

    protected virtual void InitMembers() {
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
      Check.Require(null != team, "team cannot be null");

      if (Meets.Any()) {
        throw new Exception("This division already has a schedule, teams cannot be added or removed");
      }

      if (!teams.Contains(team)) {
        teams.Add(team);
        team.Division = this;
      }
    }

    public virtual void RemoveTeam(Team team) {
      Check.Require(null != team, "team cannot be null");

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

    //public virtual void AddMeet(Meet meet) {
    //  Check.Require(null != meet, "meet cannot be null");

    //  if (!meets.Contains(meet)) {
    //    meets.Add(meet);
    //  }
    //}

    //public virtual void RemoveMeet(Meet meet) {
    //  if (null != meet && meets.Contains(meet)) {
    //    meets.Remove(meet);
    //  }
    //}

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

    public virtual void CreateSchedule(IDivisionRepository divisionRepository) {
      Check.Require(null != divisionRepository, "divisionRepository cannot be null");
      divisionRepository.Refresh(this);
      if (meets.Any()) {
        throw new CreateScheduleException("A schedule for this division already exists");
      }
      lock (scheduleLock) {
        divisionRepository.Refresh(this);
        if (meets.Any()) {
          throw new CreateScheduleException("A schedule for this division already exists");
        }

        var numTeams = teams.Count;
        if (numTeams < 2) {
          throw new ArgumentException("division must have 2 or more teams to create a schedule", "division");
        }

        var includeBye = (numTeams % 2 != 0) ? true : false;
        var numMatches = numTeams / 2;
        var numWeeks = includeBye ? numTeams : numTeams - 1;
        var opponent = -1;

        for (int i = 0; i < numWeeks; i++) {
          for (int j = 0; j < numTeams; j++) {
            if (includeBye) {
              opponent = (numTeams + i - j - 1) % numTeams;
            }
            else {
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
            }
            if (opponent != j) {
              if (!meets.Where(m => m.Teams.Contains(teams[j]) && m.Teams.Contains(teams[opponent])).Any()) {
                Meet m = new Meet(teams[j], teams[opponent], i);
                meets.Add(m);
              }
            }
          }
        }
      }
    }

  }
}
