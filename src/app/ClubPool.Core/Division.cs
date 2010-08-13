using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class Division : Entity
  {
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
    }

    public virtual DateTime StartingDate { get; set; }

    [DomainSignature]
    public virtual string Name { get; set; }

    public virtual bool CanDelete() {
      // can delete if we have no teams
      return teams.Count == 0;
    }

    [DomainSignature]
    public virtual Season Season { get; set; }

    protected IList<Team> teams;

    public virtual IEnumerable<Team> Teams { get { return teams; } }

    public virtual void AddTeam(Team team) {
      Check.Require(null != team, "team cannot be null");

      if (!teams.Contains(team)) {
        teams.Add(team);
        team.Division = this;
      }
    }

    public virtual void RemoveTeam(Team team) {
      Check.Require(null != team, "team cannot be null");

      if (teams.Contains(team)) {
        teams.Remove(team);
        team.Division = null;
      }
    }

    public virtual void RemoveAllTeams() {
      teams.Clear();
    }
  }
}
