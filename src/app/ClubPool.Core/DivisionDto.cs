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
      Name = division.Name;
      StartingDate = division.StartingDate;
      Periodicity = division.Periodicity;
      CanDelete = division.CanDelete();
    }

    private void InitMembers() {
    }

    public void UpdateDivision(Division division) {
      division.Name = Name;
      division.StartingDate = StartingDate;
      division.Periodicity = Periodicity;
    }

    [DisplayName("Starting date")]
    [NotNull]
    public DateTime StartingDate { get; set; }

    [DisplayName("Periodicity")]
    [NotNull]
    public TimeSpan Periodicity { get; set; }

    [DisplayName("Name")]
    [NotNullNotEmpty]
    public string Name { get; set; }

    public bool CanDelete { get; set; }
  }
}
