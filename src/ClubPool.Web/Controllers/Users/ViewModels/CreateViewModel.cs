using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class CreateViewModel : UserViewModelBase
  {
    [DisplayName("Password:")]
    [Required(ErrorMessage = "Required")]
    public string Password { get; set; }

    [DisplayName("Confirm password:")]
    [Required(ErrorMessage = "Required")]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
  }
}
