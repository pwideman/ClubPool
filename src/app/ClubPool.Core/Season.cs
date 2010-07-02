using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
      }
    }

    public virtual void AddDivision(Division division) {
      Check.Require(null != division, "division cannot be null");

      if (!divisions.Contains(division)) {
        divisions.Add(division);
      }
    }

    public virtual bool CanDelete() {
      return !IsActive;
    }
  }
}
