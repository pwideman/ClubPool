using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class Season : VersionedEntity
  {
    [Required]
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<Division> Divisions { get; private set; }
    public int GameTypeValue { get; set; }

    public GameType GameType {
      get {
        return (GameType)GameTypeValue;
      }
      set {
        GameTypeValue = (int)value;
      }
    }

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
      Divisions = new HashSet<Division>();
    }

    public virtual void RemoveDivision(Division division) {
      Arg.NotNull(division, "division");

      if (Divisions.Contains(division)) {
        Divisions.Remove(division);
        division.Season = null;
      }
    }

    public virtual void RemoveAllDivisions() {
      foreach (var division in Divisions) {
        division.Season = null;
      }
      Divisions.Clear();
    }

    public virtual void AddDivision(Division division) {
      Arg.NotNull(division, "division");

      if (!Divisions.Contains(division)) {
        Divisions.Add(division);
        division.Season = this;
      }
    }

    public virtual bool CanDelete() {
      // can only delete if this is not the active season and it has divisions that can't be deleted
      return !IsActive && !Divisions.Where(d => !d.CanDelete()).Any();
    }

    public virtual bool DivisionNameIsInUse(string name) {
      return Divisions.Where(d => d.Name.Equals(name)).Any();
    }
  }
}
