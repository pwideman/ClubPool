﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using SharpArch.Core;
using SharpArch.Core.DomainModel;

namespace ClubPool.Core
{
  public class Season : Entity, IEntityWithVersion
  {
    protected IList<Division> divisions;

    [DomainSignature]
    public virtual string Name { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual int Version { get; protected set; }
    public virtual IEnumerable<Division> Divisions { get { return divisions; } }
    public virtual GameType GameType { get; set; }

    protected Season() {
      InitMembers();
    }

    public Season(string name, GameType gameType)
      : this() {
      Check.Require(!string.IsNullOrEmpty(name), "name cannot be null or empty");

      Name = name;
      GameType = gameType;
    }

    protected virtual void InitMembers() {
      divisions = new List<Division>();
    }

    public virtual void RemoveDivision(Division division) {
      Check.Require(null != division, "division cannot be null");

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
      Check.Require(null != division, "division cannot be null");

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
