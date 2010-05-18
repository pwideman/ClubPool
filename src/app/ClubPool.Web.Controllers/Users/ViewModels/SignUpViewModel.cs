﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Cfg.Loquacious;
using xVal.Rules;

using ClubPool.Framework.Validation;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  [Compare(Message="Must equal Password", 
    PrimaryPropertyName="ConfirmPassword", 
    PropertyToCompare="Password",
    Operator=xVal.Rules.ComparisonRule.Operator.Equals)]
  public class SignUpViewModel : ValidatableViewModel
  {
    [DisplayName("Username:")]
    [NotNullNotEmpty(Message= "Required")]
    public string Username { get; set; }

    [DisplayName("Password:")]
    [NotNullNotEmpty(Message= "Required")]
    public string Password { get; set; }

    [DisplayName("Confirm password:")]
    [NotNullNotEmpty(Message= "Required")]
    public string ConfirmPassword { get; set; }

    [DisplayName("First")]
    [NotNullNotEmpty(Message="Required")]
    public string FirstName { get; set; }
    
    [DisplayName("Last")]
    [NotNullNotEmpty(Message="Required")]
    public string LastName { get; set; }

    [DisplayName("Email address:")]
    [NotNullNotEmpty(Message="Required")]
    [Email(Message="Enter a valid email address")]
    public string Email { get; set; }

    public string PreviousUsername { get; set; }
  }
}
