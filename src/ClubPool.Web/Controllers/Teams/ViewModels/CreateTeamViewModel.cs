using System;

using NHibernate.Validator.Constraints;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class CreateTeamViewModel : TeamViewModel
  {
    public CreateTeamViewModel()
      : base() {
    }

    public CreateTeamViewModel(IRepository repository, Division division) : base(repository, division.Season) {
      DivisionId = division.Id;
      DivisionName = division.Name;
    }

    [Min(1)]
    public int DivisionId { get; set; }

    public string DivisionName { get; set; }
  }
}
