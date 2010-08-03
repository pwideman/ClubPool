using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class PlayerViewModel : EntityViewModelBase
  {
    public PlayerViewModel()
      : base() {
    }

    public PlayerViewModel(User player)
      : base(player) {

      Name = player.FullName;
      Username = player.Username;
      Email = player.Email;
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
  }
}
