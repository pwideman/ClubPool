using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Core
{
  public class DivisionDto : EntityDto
  {
    public DivisionDto() {
      InitMembers();
    }

    public DivisionDto(Division division)
      : this() {
      Id = division.Id;
      Name = division.Name;
      StartingDate = division.StartingDate;
      CanDelete = division.CanDelete();
    }

    private void InitMembers() {
    }

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

    public bool CanDelete { get; set; }
  }
}
