using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using DataAnnotationsExtensions;

using ClubPool.Web.Controllers.Teams.ViewModels;

namespace ClubPool.Web.Controllers.Teams
{
  public abstract class TeamViewModel
  {
    [DisplayName("Players:")]
    public IEnumerable<PlayerViewModel> Players { get; set; }

    [DisplayName("Name:")]
    [Required(ErrorMessage = "Required")]
    public string Name { get; set; }

    [DisplayName("Schedule priority:")]
    [Required(ErrorMessage = "Required")]
    [Min(0, ErrorMessage = "Schedule priority must be a number greater than or equal to zero")]
    public int SchedulePriority { get; set; }

    public int[] SelectedPlayers { get; set; }
  }

  public class CreateTeamViewModel : TeamViewModel
  {
    [Min(1)]
    public int DivisionId { get; set; }
    public string DivisionName { get; set; }
  }

  public class EditTeamViewModel : TeamViewModel
  {
    [Required]
    public string Version { get; set; }

    [Min(1)]
    public int Id { get; set; }
  }

  public class PlayerViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public bool IsSelected { get; set; }
  }

  public class DetailsViewModel
  {
    public int Id { get; set; }
    public bool CanUpdateName { get; set; }
    public string Name { get; set; }
    public IEnumerable<DetailsPlayerViewModel> Players { get; set; }
    public string Record { get; set; }
    public string Rank { get; set; }
    public IEnumerable<DetailsMatchViewModel> SeasonResults { get; set; }
    public bool HasSeasonResults { get; set; }
    public string Email { get; set; }
  }

  public class DetailsPlayerViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int EightBallSkillLevel { get; set; }
  }

  public class DetailsMatchViewModel
  {
    public string TeamPlayerName { get; set; }
    public int TeamPlayerWins { get; set; }
    public string OpponentTeamName { get; set; }
    public string OpponentPlayerName { get; set; }
    public int OpponentPlayerWins { get; set; }
    public bool Win { get; set; }
  }
}