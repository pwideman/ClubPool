using System;

using ClubPool.Web.Models;

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
