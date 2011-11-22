using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

namespace ClubPool.Web.Controllers.Teams.ViewModels
{
  public class UpdateNameViewModel
  {
    [Min(1)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
  }
}
