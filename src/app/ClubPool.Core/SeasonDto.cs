using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Core
{
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
