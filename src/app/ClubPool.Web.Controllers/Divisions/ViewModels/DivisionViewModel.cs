using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Divisions.ViewModels
{

  public class DivisionViewModel : ValidatableViewModel
  {
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string SeasonName { get; set; }

    [DisplayName("Name:")]
    [NotNullNotEmpty(Message="Enter a name")]
    public string Name { get; set; }

    [DisplayName("Starting date:")]
    [NotNullNotEmpty(Message="Enter a starting date")]
    public string StartingDate { get; set; }

  }
}
