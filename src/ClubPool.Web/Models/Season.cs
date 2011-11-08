using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using ClubPool.Web.Infrastructure;
using ClubPool.Core;

namespace ClubPool.Web.Models
{
  public class Season : VersionedEntity
  {
    protected IList<Division> divisions;

    public virtual string Name { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual IEnumerable<Division> Divisions { get { return divisions; } }
    public virtual GameType GameType { get; set; }

    protected Season() {
      InitMembers();
    }

    public Season(string name, GameType gameType)
      : this() {
      Arg.NotNull(name, "name");

      Name = name;
      GameType = gameType;
    }

    protected virtual void InitMembers() {
      divisions = new List<Division>();
    }

    public virtual void RemoveDivision(Division division) {
      Arg.NotNull(division, "division");

      if (divisions.Contains(division)) {
        divisions.Remove(division);
        division.Season = null;
      }
    }

    public virtual void RemoveAllDivisions() {
      foreach (var division in divisions) {
        division.Season = null;
      }
      divisions.Clear();
    }

    public virtual void AddDivision(Division division) {
      Arg.NotNull(division, "division");

      if (!divisions.Contains(division)) {
        divisions.Add(division);
        division.Season = this;
      }
    }

    public virtual bool CanDelete() {
      // can only delete if this is not the active season and it has divisions that can't be deleted
      return !IsActive && !Divisions.Where(d => !d.CanDelete()).Any();
    }

    public virtual bool DivisionNameIsInUse(string name) {
      return divisions.Where(d => d.Name.Equals(name)).Any();
    }
  }
}
