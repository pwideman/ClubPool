using NHibernate.Validator.Constraints;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class RecoverUsernameViewModel : ValidatableViewModel
  {
    [Email]
    [NotNullNotEmpty]
    public string Email { get; set; }
  }
}
