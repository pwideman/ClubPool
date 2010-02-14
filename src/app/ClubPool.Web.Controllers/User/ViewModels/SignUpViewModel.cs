using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;

namespace ClubPool.Web.Controllers.User.ViewModels
{
  public class SignUpViewModel : ValidatableViewModel
  {
    [NotNullNotEmpty(Message="Username is missing")]
    public string Username { get; set; }

    [NotNullNotEmpty(Message="Password is missing")]
    public string Password { get; set; }

    [NotNullNotEmpty(Message="Confirmation password is missing")]
    public string ConfirmPassword { get; set; }

    [NotNullNotEmpty(Message="First name is missing")]
    public string FirstName { get; set; }

    [NotNullNotEmpty(Message="Last name is missing")]
    public string LastName { get; set; }

    [NotNullNotEmpty(Message="Email address is missing")]
    [Email(Message="Email address is invalid")]
    public string Email { get; set; }

    public string PreviousUsername { get; set; }
  }
}
