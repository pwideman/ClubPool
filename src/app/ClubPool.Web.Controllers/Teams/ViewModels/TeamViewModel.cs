using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class TeamViewModel : ValidatableViewModel
  {
    public DivisionDto Division { get; set; }
    public int Id { get; set; }

    // simple binding requires us to have an array specifically for
    // posting back the ids
    public int[] PlayerIds { get; set; }
    
    [DisplayName("Players:")]
    public UserDto[] Players { get; set; }

    [DisplayName("Available players:")]
    public UserDto[] AvailablePlayers { get; set; }

    [DisplayName("Name:")]
    [NotNullNotEmpty]
    public string Name { get; set; }
  }

}
