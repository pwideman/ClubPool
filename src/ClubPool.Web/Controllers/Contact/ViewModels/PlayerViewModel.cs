using System;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Contact.ViewModels
{
  public class PlayerViewModel : ContactViewModelBase
  {
    public PlayerViewModel() {
    }

    public PlayerViewModel(User player, User sender) {
      Id = player.Id;
      Name = player.FullName;
      ReplyToAddress = sender.Email;
    }
  }
}
