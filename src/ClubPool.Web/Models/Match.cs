using System;
using System.Linq;
using System.Collections.Generic;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class Match : Entity
  {
    public virtual Meet Meet { get; set; }
    public virtual bool IsComplete { get; set; }
    public virtual bool IsForfeit { get; set; }
    public virtual ICollection<MatchPlayer> Players { get; private set; }
    public virtual User Winner { get; set; }
    public virtual ICollection<MatchResult> Results { get; private set; }
    public virtual DateTime DatePlayed { get; set; }

    protected Match() {
      InitMembers();
    }

    public Match(Meet meet, MatchPlayer player1, MatchPlayer player2) : this() {
      Arg.NotNull(meet, "meet");
      Arg.NotNull(player1, "player1");
      Arg.NotNull(player2, "player2");
      Arg.Require(meet.Teams.Contains(player1.Team), "player1 is not a member of any of the meet's teams");
      Arg.Require(meet.Teams.Contains(player2.Team), "player2 is not a member of any of the meet's teams");

      Meet = meet;
      Players.Add(player1);
      player1.Match = this;
      Players.Add(player2);
      player2.Match = this;
    }

    protected void InitMembers() {
      Results = new List<MatchResult>();
      Players = new List<MatchPlayer>();
    }

    public virtual void AddResult(MatchResult result) {
      Arg.Require(Players.Where(p => p.Player == result.Player).Any(), "The match result must apply to one of this match's players");
      if (!Results.Contains(result)) {
        result.Match = this;
        Results.Add(result);
      }
    }

    public virtual void RemoveResult(MatchResult result) {
      if (Results.Contains(result)) {
        Results.Remove(result);
        result.Match = null;
      }
    }

    public virtual void RemoveAllResults() {
      foreach (var result in Results) {
        result.Match = null;
      }
      Results.Clear();
    }
  }
}
