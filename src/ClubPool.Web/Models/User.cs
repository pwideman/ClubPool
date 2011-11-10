using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class User : VersionedEntity
  {
    // mapped properties
    [Required]
    public string Username { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string PasswordSalt { get; set; }
    [Required]
    public string Email { get; set; }
    public bool IsApproved { get; set; }
    public bool IsLocked { get; set; }
    public virtual ICollection<Role> Roles { get; private set; }
    public virtual ICollection<SkillLevel> SkillLevels { get; private set; }

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
      Arg.NotNull(username, "username");
      Arg.NotNull(password, "password");
      Arg.NotNull(firstName, "firstName");
      Arg.NotNull(lastName, "lastName");
      //Arg.NotNull(email, "email");

      Username = username;
      Password = password;
      FirstName = firstName;
      LastName = lastName;
      Email = email;
    }

    private void InitMembers() {
      Roles = new HashSet<Role>();
      IsApproved = false;
      IsLocked = false;
      SkillLevels = new HashSet<SkillLevel>();
    }

    public virtual void RemoveAllRoles() {
      Roles.Clear();
    }

    public virtual void AddRole(Role role) {
      Arg.NotNull(role, "role");

      if (!IsInRole(role)) {
        Roles.Add(role);
      }
    }

    public virtual bool IsInRole(Role role) {
      return Roles.Contains(role);
    }

    public virtual bool IsInRole(string roleName) {
      return Roles.Where(r => r.Name.Equals(roleName)).Any();
    }

    public virtual void RemoveRole(Role role) {
      Arg.NotNull(role, "role");

      if (IsInRole(role)) {
        Roles.Remove(role);
      }
      else {
        throw new InvalidOperationException(string.Format("User does not belong to role '{0}'", role.Name));
      }
    }

    public virtual void AddSkillLevel(SkillLevel skillLevel) {
      Arg.NotNull(skillLevel, "skillLevel");

      if (!SkillLevels.Contains(skillLevel)) {
        SkillLevels.Add(skillLevel);
      }
    }

    public virtual void RemoveSkillLevel(SkillLevel skillLevel) {
      Arg.NotNull(skillLevel, "skillLevel");

      if (SkillLevels.Contains(skillLevel)) {
        SkillLevels.Remove(skillLevel);
      }
    }

    public virtual List<MatchResult> GetMatchResultsUsedInSkillLevelCalculation(GameType gameType, IRepository repository) {
      var matchResults = (from result in repository.All<MatchResult>()
                          where result.Player.Id == Id && result.Match.Meet.Division.Season.GameTypeValue == (int)gameType && !result.Match.IsForfeit
                          orderby result.Match.DatePlayed descending,
                          result.Wins > 0 ? (result.Innings - result.DefensiveShots) / result.Wins : 0 ascending,
                          result.Innings ascending
                          select result).Take(10).ToList();
      return matchResults;
    }

    public virtual void UpdateSkillLevel(GameType gameType, IRepository repository) {
      Arg.NotNull(repository, "repository");
      switch (gameType) {
        case GameType.EightBall:
          // get the last 10 matches for this player & game type
          var matchResults = GetMatchResultsUsedInSkillLevelCalculation(gameType, repository);

          if (!matchResults.Any()) {
            // this player has no valid match results for this game type, see if there's
            // an existing skill level for this game type and if so, remove it
            var sl = SkillLevels.Where(s => s.GameType == gameType).FirstOrDefault();
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
          var currentSkillLevel = SkillLevels.Where(s => s.GameType == gameType).FirstOrDefault();
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

    private int GetSkillLevelForIG(double ig) {
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

    public virtual List<MatchResult> CullTopMatchResults(List<MatchResult> matchResults) {
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

    private double CalculateIGForMatches(List<MatchResult> matches) {
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

    private class MatchResultComparer : IComparer<MatchResult>
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
