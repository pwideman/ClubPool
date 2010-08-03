using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class EditTeamViewModel : TeamViewModel
  {
    [Min(1)]
    public int Id { get; set; }
  }
}
