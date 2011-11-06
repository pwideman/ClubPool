using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class CreateSeasonViewModel : ValidatableViewModel
  {
    [DisplayName("Name:")]
    [NotNullNotEmpty(Message="Required")]
    public string Name { get; set; }
  }
}
