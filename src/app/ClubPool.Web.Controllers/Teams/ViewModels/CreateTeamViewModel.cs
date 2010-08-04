using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;
using ClubPool.Core.Contracts;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class CreateTeamViewModel : TeamViewModel
  {
    public CreateTeamViewModel()
      : base() {
    }

    public CreateTeamViewModel(IUserRepository userRepository, Division division) : base(userRepository, division.Season) {
      DivisionId = division.Id;
      DivisionName = division.Name;
    }

    [Min(1)]
    public int DivisionId { get; set; }

    public string DivisionName { get; set; }
  }
}
