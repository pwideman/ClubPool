using System;
using System.Linq;
using System.Collections.Generic;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class Match : Entity
  {
    protected IList<MatchResult> results;
    protected IList<MatchPlayer> players;

    public virtual Meet Meet { get; set; }
    public virtual bool IsComplete { get; set; }
    public virtual bool IsForfeit { get; set; }
    public virtual IEnumerable<MatchPlayer> Players { get { return players; } }
    public virtual User Winner { get; set; }
    public virtual IEnumerable<MatchResult> Results { get { return results; } }
    public virtual DateTime DatePlayed { get; set; }

    protected Match() {
      InitMembers();
    }

    public Match(Meet meet, MatchPlayer player1, MatchPlayer player2) : this() {
      Check.Require(null != meet, "meet cannot be null");
      Check.Require(null != player1, "player1 cannot be null");
      Check.Require(null != player2, "player2 cannot be null");
      Check.Require(meet.Teams.Contains(player1.Team), "player1 is not a member of any of the meet's teams");
      Check.Require(meet.Teams.Contains(player2.Team), "player2 is not a member of any of the meet's teams");

      Meet = meet;
      players.Add(player1);
      player1.Match = this;
      players.Add(player2);
      player2.Match = this;
    }

    protected void InitMembers() {
      results = new List<MatchResult>();
      players = new List<MatchPlayer>();
    }

    public virtual void AddResult(MatchResult result) {
      if (!Players.Where(p => p.Player == result.Player).Any()) {
        throw new ArgumentException("The match result must apply to one of this match's players", "result");
      }
      if (!results.Contains(result)) {
        result.Match = this;
        results.Add(result);
      }
    }

    public virtual void RemoveResult(MatchResult result) {
      if (results.Contains(result)) {
        results.Remove(result);
        result.Match = null;
      }
    }

    public virtual void RemoveAllResults() {
      foreach (var result in results) {
        result.Match = null;
      }
      results.Clear();
    }
  }
}
