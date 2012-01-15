using System;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class EditTeamPostViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Required")]
    public string Name { get; set; }

    public int[] Players { get; set; }

    [Min(0)]
    public int SchedulePriority { get; set; }

    [Required]
    public string Version { get; set; }

  }
}