using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Divisions.ViewModels
{
  public class EditDivisionViewModel : DivisionViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    [Min(1)]
    public int Version { get; set; }

    public EditDivisionViewModel()
      : base() {
    }

    public EditDivisionViewModel(Division division)
      : base(division) {
      Id = division.Id;
      Version = division.Version;
    }
  }
}
