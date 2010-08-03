using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Divisions.ViewModels
{
  public abstract class DivisionViewModel : ValidatableViewModel
  {
    public string SeasonName { get; set; }

    [DisplayName("Name:")]
    [NotNullNotEmpty(Message = "Required")]
    public string Name { get; set; }

    [DisplayName("Starting date:")]
    [NotNullNotEmpty(Message = "Required")]
    public string StartingDate { get; set; }
  }
}
