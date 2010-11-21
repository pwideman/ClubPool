using NHibernate.Validator.Constraints;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class ResetPasswordViewModel : ValidatableViewModel
  {
    [NotNullNotEmpty]
    public string Username { get; set; }
  }
}
