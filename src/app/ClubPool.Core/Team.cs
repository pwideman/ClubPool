using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using NHibernate.Validator.Constraints;
using SharpArch.Core.NHibernateValidator;
using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  [HasUniqueDomainSignature(Message = "A team already exists with this name")]
  public class Team : Entity
  {
    protected Team() {
      InitMembers();
    }

    public Team(string name, Division division):this() {
      Check.Require(!string.IsNullOrEmpty(name), "name cannot be null or empty");
      Check.Require(division != null, "division cannot be null");

      Name = name;
      Division = division;
    }

    protected virtual void InitMembers() {
      players = new List<User>();
    }

    public virtual bool CanDelete() {
      // if there are no players this team can be deleted
      return players.Count == 0;
    }

    [DomainSignature]
    [NotNullNotEmpty]
    public virtual string Name { get; set; }

    [DomainSignature]
    public virtual Division Division { get; set; }

    protected IList<User> players;

    public virtual IEnumerable<User> Players { get { return players; } }

    public virtual void RemoveAllPlayers() {
      players.Clear();
    }

    public virtual void RemovePlayer(User player) {
      Check.Require(null != player, "player cannot be null");

      if (players.Contains(player)) {
        players.Remove(player);
      }
    }

    public virtual void AddPlayer(User player) {
      Check.Require(null != player, "player cannot be null");

      if (!players.Contains(player)) {
        players.Add(player);
      }
    }
  }

  public class TeamDto : ValidatableEntityDto
  {
    public TeamDto() {
      InitMembers();
    }

    public TeamDto(Team t)
      : this() {
      Id = t.Id;
      Name = t.Name;
      Division = t.Division;
      Players = t.Players.Select(p => new UserDto(p)).ToArray();
      CanDelete = t.CanDelete();
    }

    private void InitMembers() {
      Players = new UserDto[0];
    }

    [DisplayName("Name:")]
    [NotNullNotEmpty]
    public string Name { get; set; }

    [DisplayName("Division:")]
    [NotNull]
    public Division Division { get; set; }

    [DisplayName("Players:")]
    public UserDto[] Players { get; set; }

    public bool CanDelete { get; set; }
  }
}
