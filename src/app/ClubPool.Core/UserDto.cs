using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Core
{
  public class UserDto
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

    public string FullName { get; protected set; }

    [DisplayName("Email address:")]
    [NotNullNotEmpty(Message = "Required")]
    [Email(Message="Enter a valid email address")]
    public string Email { get; set; }

    [DisplayName("Approved")]
    public bool IsApproved { get; set; }

    public string[] Roles { get; set; }

    public UserDto(User user) {
      if (null != user) {
        Id = user.Id;
        Username = user.Username;
        FirstName = user.FirstName;
        LastName = user.LastName;
        FullName = user.FullName;
        Email = user.Email;
        IsApproved = user.IsApproved;
        Roles = user.Roles.Select(r => r.Name).ToArray();
      }
    }
  }
}
