using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class EditSeasonViewModel : CreateSeasonViewModel
  {
    public EditSeasonViewModel() {
    }

    public EditSeasonViewModel(Season season) {
      Id = season.Id;
      Name = season.Name;
    }

    [Min(1)]
    public int Id { get; set; }
  }
}
