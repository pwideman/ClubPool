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

    public virtual Meet Meet { get; set; }
    public virtual bool IsComplete { get; set; }
    public virtual bool IsForfeit { get; set; }
    public virtual User Player1 { get; set; }
    public virtual User Player2 { get; set; }
    public virtual IEnumerable<User> Players { get { return new User[2] { Player1, Player2 }; } }
    public virtual User Winner { get; set; }
    public virtual IEnumerable<MatchResult> Results { get { return results; } }
    public virtual DateTime DatePlayed { get; set; }

    protected Match() {
      InitMembers();
    }

    public Match(Meet meet, User player1, User player2) : this() {
      Check.Require(null != meet, "meet cannot be null");
      Check.Require(null != player1, "player1 cannot be null");
      Check.Require(null != player2, "player2 cannot be null");
      Check.Require(meet.Teams.Where(t => t.Players.Contains(player1)).Any(), "player1 is not a member of any of the meet's teams");
      Check.Require(meet.Teams.Where(t => t.Players.Contains(player2)).Any(), "player2 is not a member of any of the meet's teams");

      Meet = meet;
      Player1 = player1;
      Player2 = player2;
    }

    protected void InitMembers() {
      results = new List<MatchResult>();
    }

    public virtual void AddResult(MatchResult result) {
      if (!Players.Contains(result.Player)) {
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
