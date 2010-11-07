using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Matches.ViewModels
{
  public class EditMatchViewModel : ValidatableViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    public string Date { get; set; }
    public string Time { get; set; }

    [NotNull]
    public PlayerViewModel Player1 { get; set; }
    [NotNull]
    public PlayerViewModel Player2 { get; set; }

    [Min(1, Message="You must select a winner")]
    public int Winner { get; set; }
    public bool IsForfeit { get; set; }
  }

  public class PlayerViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    [Min(0, Message="Innings must be >= 0")]
    public int Innings { get; set; }

    [Min(0, Message="Defensive shots must be >= 0")]
    public int DefensiveShots { get; set; }

    [Range(0, 9, Message="Wins must be between 0 and 9")]
    public int Wins { get; set; }
  }
}
