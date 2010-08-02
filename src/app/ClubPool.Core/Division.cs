using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using NHibernate.Validator.Constraints;
using SharpArch.Core.NHibernateValidator;
using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class Division : Entity
  {
    protected Division() {
      InitMembers();
    }

    public Division(string name, DateTime startingDate) : this() {
      Check.Require(!string.IsNullOrEmpty(name), "Name cannot be null");
      Check.Require(null != startingDate, "Starting date cannot be null");

      Name = name;
      StartingDate = startingDate;
    }

    protected virtual void InitMembers() {
      teams = new List<Team>();
    }

    [NotNull]
    public virtual DateTime StartingDate { get; set; }

    [NotNullNotEmpty]
    public virtual string Name { get; set; }

    public virtual bool CanDelete() {
      // can delete if we have no teams
      return teams.Count == 0;
    }

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

  public class DivisionDto : EntityDto
  {
    public DivisionDto() {
      //InitMembers();
    }

    public DivisionDto(Division division)
      : this() {
      Id = division.Id;
      Name = division.Name;
      StartingDate = division.StartingDate;
      //Teams = division.Teams.Select(t => new TeamDto(t)).ToArray();
      CanDelete = division.CanDelete();
    }

    //private void InitMembers() {
    //  Teams = new TeamDto[0];
    //}

    public void UpdateDivision(Division division) {
      division.Name = Name;
      division.StartingDate = StartingDate;
    }

    [DisplayName("Starting date")]
    [NotNull]
    public DateTime StartingDate { get; set; }

    [DisplayName("Name")]
    [NotNullNotEmpty]
    public string Name { get; set; }

    //[DisplayName("Teams")]
    //public TeamDto[] Teams { get; set; }

    public bool CanDelete { get; set; }
  }

}
