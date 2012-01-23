using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

namespace ClubPool.Web.Controllers.Divisions
{
  public abstract class DivisionViewModel
  {
    public string SeasonName { get; set; }

    [DisplayName("Name:")]
    [Required(ErrorMessage = "Required")]
    public string Name { get; set; }

    [DisplayName("Starting date:")]
    [Required(ErrorMessage = "Required")]
    public string StartingDate { get; set; }
  }

  public class EditDivisionViewModel : DivisionViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    [Required]
    public string Version { get; set; }
  }

  public class CreateDivisionViewModel : DivisionViewModel
  {
    [Min(1)]
    public int SeasonId { get; set; }
  }

}