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
      CanDelete = season.CanDelete();
    }

    protected void InitMembers() {
    }

    [DisplayName("Name:")]
    [NotNullNotEmpty]
    public string Name { get; set; }

    public bool CanDelete { get; set; }
  }
}
