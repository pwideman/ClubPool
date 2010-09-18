using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class Team : Entity, IEntityWithVersion
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
    public virtual string Name { get; set; }

    [DomainSignature]
    public virtual Division Division { get; set; }

    public virtual int Version { get; protected set; }

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
}
