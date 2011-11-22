using System;

using DataAnnotationsExtensions;

namespace ClubPool.Web.Controllers.Divisions.ViewModels
{
  public class CreateDivisionViewModel : DivisionViewModel
  {
    [Min(1)]
    public int SeasonId { get; set; }
  }
}
