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
    public TeamViewModel() {
    }

    public TeamViewModel(Team team, User sender) {
      Id = team.Id;
      Name = team.Name;
      ReplyToAddress = sender.Email;
    }
  }
}
