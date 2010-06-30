using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Core
{
  public class UserDto : EntityDto
  {
    public UserDto() {
      InitMembers();
    }

    public UserDto(User user):this() {
      if (null != user) {
        Id = user.Id;
        Username = user.Username;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        IsApproved = user.IsApproved;
        IsLocked = user.IsLocked;
        CanDelete = user.CanDelete();
        Roles = user.Roles.Select(r => r.Id).ToArray();
        RoleNames = user.Roles.Select(r => r.Name).ToArray();
      }
    }

    public void UpdateUser(User user) {
      user.Email = Email;
      user.Username = Username;
      user.FirstName = FirstName;
      user.LastName = LastName;
      user.IsApproved = IsApproved;
    }

    private void InitMembers() {
      Roles = new int[0];
    }

    [DisplayName("Username:")]
    [NotNullNotEmpty(Message = "Required")]
    public string Username { get; set; }

    [DisplayName("First")]
    [NotNullNotEmpty(Message = "Required")]
    public string FirstName { get; set; }

    [DisplayName("Last")]
    [NotNullNotEmpty(Message = "Required")]
    public string LastName { get; set; }

    public string FullName { 
      get {
        return string.Format("{0} {1}", FirstName, LastName);
      }
    }

    [DisplayName("Email address:")]
    [NotNullNotEmpty(Message = "Required")]
    [Email(Message="Enter a valid email address")]
    public string Email { get; set; }

    [DisplayName("Approved")]
    public bool IsApproved { get; set; }

    [DisplayName("Locked")]
    public bool IsLocked { get; set; }

    public bool CanDelete { get; set; }

    public int[] Roles { get; set; }

    public string[] RoleNames { get; set; }

  }
}
