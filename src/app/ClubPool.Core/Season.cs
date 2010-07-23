using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;
using SharpArch.Core;
using SharpArch.Core.DomainModel;
using SharpArch.Core.NHibernateValidator;

namespace ClubPool.Core
{
  [HasUniqueDomainSignature(Message="A season with this name already exists")]
  public class Season : Entity
  {
    protected Season() {
      InitMembers();
    }

    public Season(string name)
      : this() {
      Check.Require(!string.IsNullOrEmpty(name), "name cannot be null or empty");

      Name = name;
    }

    protected virtual void InitMembers() {
      divisions = new List<Division>();
    }

    [DomainSignature]
    [NotNullNotEmpty]
    public virtual string Name { get; set; }

    public virtual bool IsActive { get; set; }

    protected IList<Division> divisions;

    public virtual IEnumerable<Division> Divisions { get { return divisions; } }

    public virtual void RemoveDivision(Division division) {
      Check.Require(null != division, "division cannot be null");

      if (divisions.Contains(division)) {
        divisions.Remove(division);
        division.Season = null;
      }
    }

    public virtual void RemoveAllDivisions() {
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
      return !IsActive && divisions.Count == 0;
    }
  }

  public class SeasonDto : ValidatableEntityDto
  {
    public SeasonDto() {
      InitMembers();
    }

    public SeasonDto(Season season)
      : this() {
      Id = season.Id;
      Name = season.Name;
      IsActive = season.IsActive;
      if (season.Divisions.Any()) {
        Divisions = season.Divisions.Select(d => new DivisionDto(d)).OrderBy(d => d.StartingDate).ToArray();
      }
      CanDelete = season.CanDelete();
    }

    protected void InitMembers() {
      Divisions = new DivisionDto[0];
    }

    [DisplayName("Name:")]
    [NotNullNotEmpty]
    public string Name { get; set; }

    [DisplayName("Active")]
    public virtual bool IsActive { get; set; }

    public bool CanDelete { get; set; }

    public DivisionDto[] Divisions { get; set; }
  }

}
