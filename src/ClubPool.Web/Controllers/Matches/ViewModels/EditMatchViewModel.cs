using System;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

namespace ClubPool.Web.Controllers.Matches.ViewModels
{
  public class EditMatchViewModel
  {
    [Min(1, ErrorMessage="Id required")]
    public int Id { get; set; }

    public string Date { get; set; }
    public string Time { get; set; }
    public bool IsForfeit { get; set; }

    [Min(1, ErrorMessage = "You must select a winner")]
    public int Winner { get; set; }

    [Min(1, ErrorMessage = "Player 1 Id required")]
    public int Player1Id { get; set; }

    [Min(0, ErrorMessage = "Player 1 innings must be >= 0")]
    public int Player1Innings { get; set; }

    [Min(0, ErrorMessage = "Player 1 defensive shots must be >= 0")]
    public int Player1DefensiveShots { get; set; }

    [Range(0,9, ErrorMessage="Player 1 wins must be between 0 and 9")]
    public int Player1Wins { get; set; }
    
    [Min(1, ErrorMessage="Player 2 Id required")]
    public int Player2Id { get; set; }

    [Min(0, ErrorMessage = "Player 2 innings must be >= 0")]
    public int Player2Innings { get; set; }

    [Min(0, ErrorMessage = "Player 2 defensive shots must be >= 0")]
    public int Player2DefensiveShots { get; set; }

    [Range(0, 9, ErrorMessage = "Player 2 wins must be between 0 and 9")]
    public int Player2Wins { get; set; }
  }
}
