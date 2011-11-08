using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Framework.Validation;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  [Compare(Message = "Passwords do not match",
    PrimaryPropertyName = "ConfirmPassword",
    PropertyToCompare = "Password",
    Operator = xVal.Rules.ComparisonRule.Operator.Equals)]
  public class CreateViewModel : UserViewModelBase
  {
    [DisplayName("Password:")]
    [NotNullNotEmpty(Message = "Required")]
    public string Password { get; set; }

    [DisplayName("Confirm password:")]
    [NotNullNotEmpty(Message = "Required")]
    public string ConfirmPassword { get; set; }
  }
}
