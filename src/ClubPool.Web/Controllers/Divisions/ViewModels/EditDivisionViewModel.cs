using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Divisions.ViewModels
{
  public class EditDivisionViewModel : DivisionViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    [Required]
    public string Version { get; set; }

    public EditDivisionViewModel()
      : base() {
    }

    public EditDivisionViewModel(Division division)
      : base(division) {
      Id = division.Id;
      Version = division.EncodedVersion;
    }
  }
}
