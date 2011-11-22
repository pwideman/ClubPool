using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public abstract class UserViewModelBase
  {
    [DisplayName("Username:")]
    [Required(ErrorMessage = "Required")]
    public string Username { get; set; }

    [DisplayName("First")]
    [Required(ErrorMessage = "Required")]
    public string FirstName { get; set; }

    [DisplayName("Last")]
    [Required(ErrorMessage = "Required")]
    public string LastName { get; set; }

    [DisplayName("Email address:")]
    [Required(ErrorMessage = "Required")]
    [Email(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; }
  }
}
