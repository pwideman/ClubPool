using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Contact.ViewModels
{
  public class TeamViewModel : ContactViewModelBase
  {
    [Min(1)]
    public int TeamId { get; set; }
    public string TeamName { get; set; }

    public TeamViewModel() {
    }

    public TeamViewModel(Team team, User sender) {
      TeamId = team.Id;
      TeamName = team.Name;
      ReplyToAddress = sender.Email;
    }
  }
}
