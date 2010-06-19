using System;
using System.Collections.Generic;

using NHibernate.Validator.Constraints;
using SharpArch.Core.NHibernateValidator;
using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  [HasUniqueDomainSignature(Message = "A user already exists with this username")]
  public class User : Entity
  {
    protected User() {
      InitMembers();
    }

    public User(string username, string password, string firstName, string lastName, string email)
      : this() {
      Check.Require(!string.IsNullOrEmpty(username), "username cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(password), "password cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(email), "email cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(firstName), "firstName cannot be null or empty");
      Check.Require(!string.IsNullOrEmpty(lastName), "lastName cannot be null or empty");

      Username = username;
      Password = password;
      FirstName = firstName;
      LastName = lastName;
      Email = email;
    }

    protected virtual void InitMembers() {
      roles = new List<Role>();
      IsApproved = false;
      IsLocked = false;
    }

    [DomainSignature]
    [NotNullNotEmpty]
    public virtual string Username { get; set; }

    [NotNullNotEmpty]
    public virtual string FirstName { get; set; }

    [NotNullNotEmpty]
    public virtual string LastName { get; set; }

    public virtual string FullName { 
      get { 
        return string.Format("{0} {1}", FirstName, LastName);
      }
    }


    [NotNullNotEmpty]
    public virtual string Password { get; set; }

    public virtual string PasswordSalt { get; set; }

    [Email]
    public virtual string Email { get; set; }

    public virtual bool IsApproved { get; set; }

    public virtual bool IsLocked { get; set; }

    protected IList<Role> roles;

    public virtual IEnumerable<Role> Roles { get { return roles; } }

    public virtual User RemoveAllRoles() {
      roles.Clear();
      return this;
    }

    public virtual User AddRole(Role role) {
      Check.Require(null != role, "role cannot be null");

      roles.Add(role);
      return this;
    }

    public virtual bool IsInRole(Role role) {
      return roles.Contains(role);
    }

    public virtual User RemoveRole(Role role) {
      Check.Require(null != role, "role cannot be null");

      if (IsInRole(role)) {
        roles.Remove(role);
      }
      else {
        throw new InvalidOperationException(string.Format("User does not belong to role '{0}'", role.Name));
      }
      return this;
    }

  }
}
