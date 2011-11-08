using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class UpdateNameViewModel : ValidatableViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    [NotNullNotEmpty]
    public string Name { get; set; }
  }
}
