using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Cfg.Loquacious;
using xVal.Rules;

using ClubPool.Framework.Validation;

namespace ClubPool.Web.Controllers.User.ViewModels
{
  [Compare(Message="Must equal Password", 
    PrimaryPropertyName="ConfirmPassword", 
    PropertyToCompare="Password",
    Operator=xVal.Rules.ComparisonRule.Operator.Equals)]
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

    public string ErrorMessage { get; set; }
  }
}
