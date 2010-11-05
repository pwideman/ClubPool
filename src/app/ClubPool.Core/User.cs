using System;
using System.Collections.Generic;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class User : Entity, IEntityWithVersion
  {
    // fields
    protected IList<Role> roles;
    protected IList<SkillLevel> skillLevels;

    // mapped properties
    [DomainSignature]
    public virtual string Username { get; set; }
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual string Password { get; set; }
    public virtual string PasswordSalt { get; set; }
    public virtual string Email { get; set; }
    public virtual bool IsApproved { get; set; }
    public virtual bool IsLocked { get; set; }
    public virtual int Version { get; protected set; }
    public virtual IEnumerable<Role> Roles { get { return roles; } }
    public virtual IEnumerable<SkillLevel> SkillLevels { get { return skillLevels; } }

    // generated properties
    public virtual string FullName {
      get {
        return string.Format("{0} {1}", FirstName, LastName);
      }
    }

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

    public virtual void RemoveAllRoles() {
      roles.Clear();
    }

    public virtual void AddRole(Role role) {
      Check.Require(null != role, "role cannot be null");

      if (!IsInRole(role)) {
        roles.Add(role);
      }
    }

    public virtual bool IsInRole(Role role) {
      return roles.Contains(role);
    }

    public virtual void RemoveRole(Role role) {
      Check.Require(null != role, "role cannot be null");

      if (IsInRole(role)) {
        roles.Remove(role);
      }
      else {
        throw new InvalidOperationException(string.Format("User does not belong to role '{0}'", role.Name));
      }
    }

    public virtual bool CanDelete() {
      return true;
    }

    public virtual void AddSkillLevel(SkillLevel skillLevel) {
      Check.Require(null != skillLevel, "skillLevel cannot be null");

      if (!skillLevels.Contains(skillLevel)) {
        skillLevels.Add(skillLevel);
      }
    }

    public virtual void RemoveSkillLevel(SkillLevel skillLevel) {
      Check.Require(null != skillLevel, "skillLevel cannot be null");

      if (skillLevels.Contains(skillLevel)) {
        skillLevels.Remove(skillLevel);
      }
    }

    public virtual int UpdateSkillLevel(GameType gameType) {
      //select TOP 10 * from Matches Where  (Player1Wins <> 0 OR PLayer2Wins <> 0 OR Player1Innings <> 0 OR Player2Innings <> 0) And 
      //(Player1 = ? Or Player2 = ?) And IsCompleted=TRUE And IsVerifiedByP1=TRUE And IsVerifiedByP2=TRUE And DatePlayed <> null Order By DatePlayed DESC
      return 0;
    }

  }
}
