using System;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class EditSeasonViewModel : CreateSeasonViewModel
  {
    public EditSeasonViewModel() {
    }

    public EditSeasonViewModel(Season season) {
      Id = season.Id;
      Name = season.Name;
      Version = season.EncodedVersion;
    }

    [Min(1)]
    public int Id { get; set; }

    [Required]
    public string Version { get; set; }
  }
}
