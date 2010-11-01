using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Matches.ViewModels
{
  public class EditMatchViewModel
  {
    public int Id { get; set; }
    public PlayerViewModel Player1 { get; set; }
    public PlayerViewModel Player2 { get; set; }
    public int Winner { get; set; }
  }

  public class PlayerViewModel
  {
    public int Id { get; set; }
    public int Innings { get; set; }
    public int DefensiveShots { get; set; }
    public int Wins { get; set; }
  }
}
