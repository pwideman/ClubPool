using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Divisions.ViewModels
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

    public DivisionViewModel() {
    }

    public DivisionViewModel(Division division) {
      SeasonName = division.Season.Name;
      Name = division.Name;
      StartingDate = division.StartingDate.ToShortDateString();
    }
  }
}
