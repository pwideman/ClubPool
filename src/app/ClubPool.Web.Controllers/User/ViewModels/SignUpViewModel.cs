using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

namespace ClubPool.Web.Controllers.User.ViewModels
{
  public class SignUpViewModel : ValidatableViewModel
  {
    [NotNullNotEmpty(Message= "Required")]
    public string Username { get; set; }

    [NotNullNotEmpty(Message= "Required")]
    public string Password { get; set; }

    [NotNullNotEmpty(Message= "Required")]
    public string ConfirmPassword { get; set; }

    [NotNullNotEmpty(Message="Required")]
    public string FirstName { get; set; }

    [NotNullNotEmpty(Message="Required")]
    public string LastName { get; set; }

    [NotNullNotEmpty(Message="Required")]
    [Email(Message="Enter a valid email address")]
    public string Email { get; set; }

    public string PreviousUsername { get; set; }
  }
}
