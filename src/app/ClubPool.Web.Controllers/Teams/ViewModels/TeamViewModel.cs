using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public abstract class TeamViewModel : ValidatableViewModel
  {
    protected TeamViewModel() {
      InitMembers();
    }

    protected void InitMembers() {
      Players = new List<PlayerViewModel>();
      AvailablePlayers = new List<PlayerViewModel>();
    }

    [DisplayName("Players:")]
    public IEnumerable<PlayerViewModel> Players { get; set; }

    [DisplayName("Available players:")]
    public IEnumerable<PlayerViewModel> AvailablePlayers { get; set; }

    [DisplayName("Name:")]
    [NotNullNotEmpty]
    public string Name { get; set; }
  }

}
