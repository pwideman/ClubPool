using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class RecoverUsernameViewModel
  {
    [Email]
    [Required]
    public string Email { get; set; }
  }
}
