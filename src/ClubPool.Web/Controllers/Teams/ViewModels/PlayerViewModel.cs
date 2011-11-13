using System;
using System.Collections.Generic;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class PlayerViewModel : EntityViewModelBase
  {
    public PlayerViewModel()
      : base() {
    }

    public User Player {
      set {
        Id = value.Id;
        Name = value.FullName;
        Username = value.Username;
        Email = value.Email;
      }
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
  }
}
