using System;
using System.Collections.Generic;
using System.Linq;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

using ClubPool.Core.Contracts;

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
      skillLevels = new List<SkillLevel>();
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

    public virtual void UpdateSkillLevel(GameType gameType, IMatchResultRepository matchResultRepository) {
      switch (gameType) {
        case GameType.EightBall:
          // get the last 10 matches for this player & game type
          var matchResults = (from result in matchResultRepository.GetMatchResultsForPlayerAndGameType(this, gameType)
                              orderby result.Innings ascending
                              orderby (result.Innings - result.DefensiveShots) / result.Wins ascending
                              orderby result.Match.DatePlayed descending
                              select result).Take(10).ToList();

          if (!matchResults.Any()) {
            // this player has no valid match results for this game type, see if there's
            // an existing skill level for this game type and if so, remove it
            var sl = skillLevels.Where(s => s.GameType == gameType).FirstOrDefault();
            if (null != sl) {
              RemoveSkillLevel(sl);
            }
            return;
          }

          // this logic and all of these helper functions are from the previous code,
          // I'm sure it can be improved upon
          var culledMatches = CullTopMatchResults(matchResults);
          var ig = CalculateIGForMatches(culledMatches);
          var skillLevel = GetSkillLevelForIG(ig);
          var currentSkillLevel = skillLevels.Where(s => s.GameType == gameType).FirstOrDefault();
          if (null != currentSkillLevel) {
            currentSkillLevel.Value = skillLevel;
          }
          else {
            AddSkillLevel(new SkillLevel(this, gameType, skillLevel));
          }
          break;

        default:
          // unknown game type
          throw new ArgumentException("Unknown game type", "gameType");
      }
    }

    protected int GetSkillLevelForIG(double ig) {
      if (ig <= 2) {
        return 9;
      }
      else if (ig <= 3) {
        return 8;
      }
      else if (ig <= 4) {
        return 7;
      }
      else if (ig <= 6) {
        return 6;
      }
      else if (ig <= 8) {
        return 5;
      }
      else if (ig <= 10) {
        return 4;
      }
      else if (ig <= 15) {
        return 3;
      }
      else {
        return 2;
      }
    }

    protected List<MatchResult> CullTopMatchResults(List<MatchResult> matchResults) {
      matchResults.Sort(new MatchResultComparer());
      int numToCount = Math.Min((int)Math.Ceiling((double)matchResults.Count / (double)2), 5);
      List<MatchResult> listMatchesIncluded = new List<MatchResult>();
      List<MatchResult> tempIncludedList = new List<MatchResult>();
      List<MatchResult> listMatchesExcluded = new List<MatchResult>();
      foreach (MatchResult m in matchResults) {
        if (listMatchesIncluded.Count < numToCount) {
          listMatchesIncluded.Add(m);
          tempIncludedList.Add(m);
        }
        else {
          listMatchesExcluded.Add(m);
        }
      }
      double ig = CalculateIGForMatches(listMatchesIncluded);
      int pos = numToCount - 1;
      foreach (MatchResult m in listMatchesExcluded) {
        if (m.Innings < listMatchesIncluded[pos].Innings) {
          tempIncludedList[pos] = m;
          double tempIG = CalculateIGForMatches(tempIncludedList);
          if (tempIG > ig) {
            break;
          }
          else {
            listMatchesIncluded[pos--] = m;
            ig = tempIG;
          }
        }
        else {
          break;
        }
      }
      return listMatchesIncluded;
    }

    protected double CalculateIGForMatches(List<MatchResult> matches) {
      double totalInnings = 0, totalWins = 0;
      foreach (MatchResult m in matches) {
        totalInnings += (m.Innings-m.DefensiveShots);
        totalWins += m.Wins;
      }
      if (0 == totalWins) {
        return Double.PositiveInfinity;
      }
      else {
        return totalInnings / totalWins;
      }
    }

    protected class MatchResultComparer : IComparer<MatchResult>
    {
      public int Compare(MatchResult m1, MatchResult m2) {
        if (null == m1) {
          if (null == m2) {
            return 0;
          }
          else {
            return 1;
          }
        }
        else {
          if (null == m2) {
            return -1;
          }
          else {
            var m1IG = m1.Wins > 0 ? (m1.Innings-m1.DefensiveShots) / m1.Wins : double.PositiveInfinity;
            var m2IG = m2.Wins > 0 ? (m2.Innings-m2.DefensiveShots) / m2.Wins : double.PositiveInfinity;
            if (m1IG == m2IG) {
              if ((m2.Innings-m2.DefensiveShots) < (m1.Innings-m1.DefensiveShots)) {
                return 1;
              }
              else if ((m1.Innings - m1.DefensiveShots) < (m2.Innings - m2.DefensiveShots)) {
                return -1;
              }
              else {
                return 0;
              }
            }
            else if (m2IG < m1IG) {
              return 1;
            }
            else {
              return -1;
            }
          }
        }
      }
    }

  }
}
