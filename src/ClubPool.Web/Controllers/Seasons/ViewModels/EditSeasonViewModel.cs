using System;

using NHibernate.Validator.Constraints;

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

    [NotNullNotEmpty]
    public string Version { get; set; }
  }
}
