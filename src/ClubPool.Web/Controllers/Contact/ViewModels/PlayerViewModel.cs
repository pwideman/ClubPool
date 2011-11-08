using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

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
