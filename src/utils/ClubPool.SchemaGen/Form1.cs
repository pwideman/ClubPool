using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using SharpArch.Data.NHibernate;
using NHibernate.Tool.hbm2ddl;

using Core=ClubPool.Core;
using ClubPool.Data;
using ClubPool.Data.NHibernateMaps;
using ClubPool.ApplicationServices.Membership;
using ClubPool.Framework.NHibernate;

namespace ClubPool.SchemaGen
{
  public partial class SchemaGen : Form
  {
    protected long beginTicks = 0;
    protected StringWriter outputStream;

    public SchemaGen() {
      InitializeComponent();
      outputStream = new StringWriter(new StringBuilder(OutputTextBox.Text));
    }

    private void button1_Click(object sender, EventArgs e) {
      try {
        beginTicks = DateTime.Now.Ticks;

        initializeNH();
        
        var userRepo = new UserRepository();
        var membershipService = new SharpArchMembershipService(userRepo);

        output("Creating dummy data");
        int userIndex = 1;
        var users = new List<Core.User>();
        using (userRepo.DbContext.BeginTransaction()) {
          for (userIndex = 1; userIndex <= 60; userIndex++) {
            var username = "user " + userIndex.ToString();
            users.Add(membershipService.CreateUser(username, "user", "user", userIndex.ToString(), 
              "user" + userIndex.ToString() + "@email.com", true, false));
          }
          userRepo.DbContext.CommitTransaction();
        }

        var seasonRepo = new SeasonRepository();
        using (seasonRepo.DbContext.BeginTransaction()) {
          for (int seasonIndex = 1; seasonIndex <= 5; seasonIndex++) {
            output("Creating season " + seasonIndex.ToString());
            var season = new Core.Season("Season " + seasonIndex.ToString(), Core.GameType.EightBall);
            season.IsActive = false;
            userIndex = 0;
            for (int divisionIndex = 1; divisionIndex <= 2; divisionIndex++) {
              output("Creating division " + divisionIndex.ToString());
              var division = new Core.Division("Division " + divisionIndex.ToString(), DateTime.Parse("1/" + divisionIndex.ToString() + "/200" + seasonIndex.ToString()), season);
              season.AddDivision(division);
              for (int teamIndex = 1; teamIndex <= 12; teamIndex++) {
                output("Creating team " + teamIndex.ToString());
                var team = new Core.Team("Team " + teamIndex.ToString(), division);
                division.AddTeam(team);
                team.AddPlayer(users[userIndex++]);
                team.AddPlayer(users[userIndex++]);
              }
            }
            seasonRepo.SaveOrUpdate(season);
          }
          var firstSeason = seasonRepo.GetAll().First();
          firstSeason.IsActive = true;
          seasonRepo.SaveOrUpdate(firstSeason);
          seasonRepo.DbContext.CommitTransaction();
        }
        output("Finished");
      }
      catch (Exception ex) {
        output("Exception:");
        output(getExceptionText(ex));
      }
    }

    private string getExceptionText(Exception e) {
      var text = new StringBuilder();
      text.Append(e.Message + Environment.NewLine + "Stack trace:" + Environment.NewLine + e.StackTrace);
      if (null != e.InnerException) {
        text.Append(getExceptionText(e.InnerException));
      }
      return text.ToString();
    }

    private void output(string text) {
      var elapsedTime = new TimeSpan(DateTime.Now.Ticks - beginTicks);
      outputStream.WriteLine(string.Format("{0}:{1}.{2} - {3}", 
        elapsedTime.Minutes.ToString("00"),
        elapsedTime.Seconds.ToString("00"),
        elapsedTime.Milliseconds, text));
      //OutputTextBox.Update();
    }

    private void button2_Click(object sender, EventArgs e) {
      try {
        beginTicks = DateTime.Now.Ticks;

        initializeNH();

        var userRepo = new UserRepository();
        var previousInfoRepo = new LinqRepository<Core.PreviousUserAccountInfo>();
        var membershipService = new SharpArchMembershipService(userRepo, true, true);
        var divisionRepo = new DivisionRepository();

        using (var context = new ipoolEntities()) {
          var oldUserIds = new Dictionary<int, Core.User>();
          var oldUsers = new Dictionary<int, User>();
          using (userRepo.DbContext.BeginTransaction()) {
            foreach (var user in context.Users) {
              var names = user.UserName.Split('_');
              var firstName = names[0].Substring(0, 1).ToUpper() + names[0].Substring(1);
              var lastName = "";
              if (names.Length > 1) {
                lastName = names[1].Substring(0, 1).ToUpper() + names[1].Substring(1);
              }
              var newUser = membershipService.CreateUser(user.UserName, user.PasswordSalt, firstName, lastName, user.Email, true, true);
              var previousInfo = new Core.PreviousUserAccountInfo(newUser, user.Password, user.PasswordSalt, user.UserId);
              previousInfoRepo.SaveOrUpdate(previousInfo);
              output(string.Format("Migrated user '{0}'", newUser.Username));
              oldUserIds.Add(user.UserId, newUser);
              oldUsers.Add(newUser.Id, user);
            }

            var seasonRepo = new SeasonRepository();
            foreach (var season in context.Seasons) {
              output(string.Format("Beginning migration for season '{0}'", season.Year.ToString()));

              var seasonName = "8-ball " + season.Year.ToString();
              var newSeason = new Core.Season(seasonName, Core.GameType.EightBall);
              seasonRepo.SaveOrUpdate(newSeason);
              output("Season created, migrating divisions");

              foreach (var division in season.Divisions) {
                if (division.Description != "HistoricalDummyDivision") {
                  var newDivision = new Core.Division(division.Description, season.StartDate.Value.AddDays(division.DateOffset), newSeason);
                  newSeason.AddDivision(newDivision);
                  output(string.Format("Added division '{0}', migrating teams", newDivision.Name));

                  var oldTeamIds = new Dictionary<Core.Team, int>();
                  foreach (var team in division.Teams) {
                    var newTeam = new Core.Team(team.Name, newDivision);
                    newTeam.AddPlayer(oldUserIds[team.Player1ID]);
                    newTeam.AddPlayer(oldUserIds[team.Player2ID]);
                    newDivision.AddTeam(newTeam);
                    oldTeamIds.Add(newTeam, team.ID);
                    output(string.Format("Added team '{0}'", newTeam.Name));
                  }

                  output("Creating schedule");
                  newDivision.CreateSchedule(divisionRepo);
                  output("Migrating matches");
                  foreach (var meet in newDivision.Meets) {
                    var team1Id = oldTeamIds[meet.Team1];
                    var team2Id = oldTeamIds[meet.Team2];
                    var matches = division.Matches.Where(m => (m.Team1Id == team1Id && m.Team2Id == team2Id) || (m.Team1Id == team2Id && m.Team2Id == team1Id));
                    int i = 0;
                    foreach (var match in matches) {
                      output(string.Format("Migrating old match '{0}'", match.ID.ToString()));
                      var player1 = oldUserIds[match.Player1Id];
                      var player2 = oldUserIds[match.Player2Id];
                      var newMatch = meet.Matches.ElementAt(i++);
                      newMatch.Player1 = player1;
                      newMatch.Player2 = player2;
                      if (match.Player1Wins == 0 && match.Player2Wins == 0) {
                        // forfeit
                        newMatch.IsForfeit = true;
                        output("Match is a forfeit");
                      }
                      else {
                        output("Adding results");
                        newMatch.AddResult(new Core.MatchResult(player1, match.Player1Innings, 0, match.Player1Wins));
                        newMatch.AddResult(new Core.MatchResult(player2, match.Player2Innings, 0, match.Player2Wins));
                        newMatch.DatePlayed = match.DatePlayed.Value;
                      }
                      newMatch.Winner = oldUserIds[match.WinnerId.Value];
                      newMatch.IsComplete = true;
                      output(string.Format("Finished match '{0}'", match.ID));
                    }
                  }
                  output(string.Format("Completed migration for division '{0}'", division.Description));
                }
              }
              output(string.Format("Completed migration for season '{0}'", newSeason.Name));
            }
            var matchResultRepo = new MatchResultRepository();
            output("Updating skill levels");
            foreach (var user in userRepo.GetAll()) {
              user.UpdateSkillLevel(Core.GameType.EightBall, matchResultRepo);
              var oldSL = 0;
              if (oldUsers.ContainsKey(user.Id)) {
                if (oldUsers[user.Id].Handicap.HasValue) {
                  oldSL = oldUsers[user.Id].Handicap.Value;
                }
              }
              var newSL = 0;
              if (user.SkillLevels.Any()) {
                newSL = user.SkillLevels.First().Value;
              }
              if (newSL != oldSL) {
                output(string.Format("Different skill level for user '{0}', old: {1} new {2}", user.FullName, oldSL, newSL));
              }
            }
            output("Committing transaction");
            userRepo.DbContext.CommitTransaction();
            output("Finished");
          }
        }

      }
      catch (Exception ex) {
        output("Exception:");
        output(getExceptionText(ex));
      }
    }

    private void initializeNH() {
      var mappingAssemblies = new string[] { "ClubPool.Data.dll" };
      output("Creating NH configuration...");
      var configuration = NHibernateSession.Init(new SimpleSessionStorage(), mappingAssemblies,
                             new AutoPersistenceModelGenerator().Generate(),
                             "NHibernate.config");
      var sb = new StringBuilder();
      var sw = new StringWriter(sb);
      var session = NHibernateSession.GetDefaultSessionFactory().OpenSession();
      output("Exporting schema...");
      new SchemaExport(configuration).Execute(true, true, false, session.Connection, sw);

      sw.Flush();
      output("SchemaExport output:");
      output(sb.Replace("\n", Environment.NewLine).ToString());

      output("Creating roles");
      var roleRepo = new RoleRepository();
      Core.Role admin = null;
      Core.Role officer = null;
      using (roleRepo.DbContext.BeginTransaction()) {
        admin = new Core.Role(Core.Roles.Administrators);
        roleRepo.SaveOrUpdate(admin);
        officer = new Core.Role(Core.Roles.Officers);
        roleRepo.SaveOrUpdate(officer);
        roleRepo.DbContext.CommitTransaction();
      }

      output("Creating special users");
      var userRepo = new UserRepository();
      var membershipService = new SharpArchMembershipService(userRepo);
      using (userRepo.DbContext.BeginTransaction()) {
        // create admin user
        var user = membershipService.CreateUser("admin", "admin", "admin", "user", "admin@admin.com", true, false);
        user.AddRole(admin);
        userRepo.SaveOrUpdate(user);
        // create officer user
        user = membershipService.CreateUser("officer", "officer", "officer", "user", "officer@email.com", true, false);
        user.AddRole(officer);
        userRepo.SaveOrUpdate(user);
        userRepo.DbContext.CommitTransaction();
      }

    }
  }
}
