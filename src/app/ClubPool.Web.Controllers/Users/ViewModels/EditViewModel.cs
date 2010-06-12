using System;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Framework.Validation;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class EditViewModel : ValidatableViewModel
  {
    public int Id { get; set; }

    [DisplayName("Username:")]
    [NotNullNotEmpty(Message = "Required")]
    public string Username { get; set; }

    [DisplayName("First")]
    [NotNullNotEmpty(Message = "Required")]
    public string FirstName { get; set; }

    [DisplayName("Last")]
    [NotNullNotEmpty(Message = "Required")]
    public string LastName { get; set; }

    [DisplayName("Email address:")]
    [NotNullNotEmpty(Message = "Required")]
    [Email(Message = "Enter a valid email address")]
    public string Email { get; set; }

    [DisplayName("Approved")]
    public bool IsApproved { get; set; }
  }
}
