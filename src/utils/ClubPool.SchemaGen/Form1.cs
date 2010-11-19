using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Configuration;

using SharpArch.Data.NHibernate;
using NHibernate.Tool.hbm2ddl;
using MySql.Data.MySqlClient;

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

    public SchemaGen() {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      try {
        beginTicks = DateTime.Now.Ticks;

        initializeNH();
        CreateSpecialUsersAndRoles();

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
      OutputTextBox.Text += Environment.NewLine + string.Format("{0}:{1}.{2} - {3}", 
        elapsedTime.Minutes.ToString("00"),
        elapsedTime.Seconds.ToString("00"),
        elapsedTime.Milliseconds.ToString("000"), text);
      OutputTextBox.Update();
      OutputTextBox.ScrollToCaret();
    }

    private void button3_Click(object sender, EventArgs e) {
      try {
        beginTicks = DateTime.Now.Ticks;

        initializeNH();

        var id = 1;

        using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["cp"].ConnectionString)) {
          output("opening mysql connection");
          conn.Open();
          using (var tx = conn.BeginTransaction())
          using (var context = new ipoolEntities()) {
            var oldUserIdsToNewUserIds = new Dictionary<int, int>();
            var newUserIdsToOldUserIds = new Dictionary<int, int>();

            int nextId = 1;

            MigrateUsers(conn, tx, oldUserIdsToNewUserIds, newUserIdsToOldUserIds, context, ref nextId);

            MigrateSeasons(conn, tx, oldUserIdsToNewUserIds, newUserIdsToOldUserIds, context, ref nextId);

            UpdateNextHi(nextId, conn, tx);

            output("committing transaction");
            tx.Commit();
          }
          output("closing mysql connection");
          conn.Close();
        }

        CreateSpecialUsersAndRoles();
      }
      catch (Exception ex) {
        output("Exception:");
        output(getExceptionText(ex));
      }
    }

    private void MigrateSeasons(MySqlConnection conn,
      MySqlTransaction tx,
      Dictionary<int, int> oldIds,
      Dictionary<int, int> newIds,
      ipoolEntities context,
      ref int nextId) {

      output("Migrating seasons");

      var commandText = @"insert into seasons(id, version, name, isactive, gametype) 
                          values (@id, 1, @name, false, @gametype);";
      var seasonCmd = new MySqlCommand(commandText, conn, tx);
      seasonCmd.Prepare();
      seasonCmd.Parameters.AddWithValue("@id", 1);
      seasonCmd.Parameters.AddWithValue("@name", "name");
      seasonCmd.Parameters.AddWithValue("@gametype", Core.GameType.EightBall.ToString());
      foreach (var season in context.Seasons) {
        var seasonId = nextId++;
        seasonCmd.Parameters["@id"].Value = seasonId;
        var name = "8-ball " + season.Year.ToString();
        seasonCmd.Parameters["@name"].Value = name;
        output(string.Format("inserting season '{0}'", name));
        seasonCmd.ExecuteNonQuery();
        MigrateDivisions(season, seasonId, conn, tx, ref nextId, oldIds, newIds);
      }
    }

    private void MigrateDivisions(Season season, 
      int seasonId, 
      MySqlConnection conn, 
      MySqlTransaction tx, 
      ref int nextId,
      Dictionary<int,int> oldIds,
      Dictionary<int,int> newIds) {

      output(string.Format("Migrating divisions for season '{0}'", season.Year.ToString()));

      var commandText = @"insert into divisions (id, version, startingdate, name, seasonid)
                          values (@id, 1, @date, @name, @seasonid);";
      var cmd = new MySqlCommand(commandText, conn, tx);
      cmd.Prepare();
      cmd.Parameters.AddWithValue("@id", 1);
      cmd.Parameters.AddWithValue("@name", "name");
      cmd.Parameters.AddWithValue("@date", DateTime.Now);
      cmd.Parameters.AddWithValue("@seasonid", seasonId);
      foreach (var division in season.Divisions) {
        var divisionId = nextId++;
        cmd.Parameters["@id"].Value = divisionId;
        cmd.Parameters["@name"].Value = division.Description;
        cmd.Parameters["@date"].Value = season.StartDate.Value.AddDays(division.DateOffset);
        output(string.Format("inserting division '{0}'", division.Description));
        cmd.ExecuteNonQuery();
        MigrateTeams(division, divisionId, conn, tx, ref nextId, oldIds, newIds);
        MigrateMatches(division, divisionId, conn, tx, ref nextId, oldIds, newIds);
      }
    }

    private void MigrateMatches(Division division,
      int divisionId,
      MySqlConnection conn,
      MySqlTransaction tx,
      ref int nextId,
      Dictionary<int, int> oldIds,
      Dictionary<int, int> newIds) {

      var meetText = @"insert into meets (id, week, iscomplete, divisionid, team1id, team2id)
                       values (@id, @week, true, @divisionid, @team1id, @team2id);";

      foreach (var match in division.Matches.OrderBy(m => m.Team1Id).ThenBy(m => m.Team2Id)) {
      }
    }

    private void MigrateTeams(Division division,
      int divisionId,
      MySqlConnection conn,
      MySqlTransaction tx,
      ref int nextId,
      Dictionary<int, int> oldIds,
      Dictionary<int, int> newIds) {

      output(string.Format("Migrating teams for division {0}", division.Description));

      var commandText = @"insert into teams (id, version, name, divisionid) values (@id, 1, @name, @divisionid);";
      var cmd = new MySqlCommand(commandText, conn, tx);
      cmd.Prepare();
      cmd.Parameters.AddWithValue("@id", 1);
      cmd.Parameters.AddWithValue("@name", "name");
      cmd.Parameters.AddWithValue("@divisionid", divisionId);

      commandText = @"insert into teamsplayers (teamid, userid) values (@teamid, @userid);";
      var playerCmd = new MySqlCommand(commandText, conn, tx);
      playerCmd.Prepare();
      playerCmd.Parameters.AddWithValue("@teamid", 1);
      playerCmd.Parameters.AddWithValue("@userid", 1);
      foreach (var team in division.Teams) {
        var teamId = nextId++;
        cmd.Parameters["@id"].Value = teamId;
        cmd.Parameters["@name"].Value = team.Name;
        output(string.Format("inserting team '{0}'", team.Name));
        cmd.ExecuteNonQuery();
        
        playerCmd.Parameters["@teamid"].Value = teamId;
        playerCmd.Parameters["@userid"].Value = oldIds[team.Player1ID];
        output(string.Format("adding player1 {0}", team.Player1.UserName));
        playerCmd.ExecuteNonQuery();

        if (team.Player2ID != team.Player1ID) {
          playerCmd.Parameters["@userid"].Value = oldIds[team.Player2ID];
          output(string.Format("adding player2 {0}", team.Player2.UserName));
          playerCmd.ExecuteNonQuery();
        }
      }
    }

    private void MigrateUsers(MySqlConnection conn, 
      MySqlTransaction tx,
      Dictionary<int, int> oldIds, 
      Dictionary<int, int> newIds, 
      ipoolEntities context,
      ref int nextId) {

      output("Migrating users");

      var commandText = @"insert into users(id, version, username, firstname, lastname, email, isapproved, islocked) 
                          values (@id, 1, @username, @firstname, @lastname, @email, true, true);";
      var command = new MySqlCommand(commandText, conn, tx);
      command.Prepare();
      command.Parameters.AddWithValue("@id", 1);
      command.Parameters.AddWithValue("@username", "username");
      command.Parameters.AddWithValue("@firstname", "firstname");
      command.Parameters.AddWithValue("@lastname", "lastname");
      command.Parameters.AddWithValue("@email", "email");

      commandText = @"insert into previoususeraccountinfos(id, password, salt, previoususerid, userid)
                      values (@id, @password, @salt, @previoususerid, @userid);";
      var infoCommand = new MySqlCommand(commandText, conn, tx);
      infoCommand.Prepare();
      infoCommand.Parameters.AddWithValue("@id", 1);
      infoCommand.Parameters.AddWithValue("@password", "password");
      infoCommand.Parameters.AddWithValue("@salt", "salt");
      infoCommand.Parameters.AddWithValue("@previoususerid", "previd");
      infoCommand.Parameters.AddWithValue("@userid", "userid");

      foreach (var user in context.Users) {
        var names = user.UserName.Split('_');
        var firstName = names[0].Substring(0, 1).ToUpper() + names[0].Substring(1);
        var lastName = "";
        if (names.Length > 1) {
          lastName = names[1].Substring(0, 1).ToUpper() + names[1].Substring(1);
        }
        var newUserId = nextId++;
        command.Parameters["@id"].Value = newUserId;
        command.Parameters["@username"].Value = user.UserName;
        command.Parameters["@firstname"].Value = firstName;
        command.Parameters["@lastname"].Value = lastName;
        command.Parameters["@email"].Value = user.Email;
        output("inserting user " + user.UserName);
        command.ExecuteNonQuery();

        infoCommand.Parameters["@id"].Value = nextId++;
        infoCommand.Parameters["@password"].Value = user.Password;
        infoCommand.Parameters["@salt"].Value = user.PasswordSalt;
        infoCommand.Parameters["@previoususerid"].Value = user.UserId;
        infoCommand.Parameters["@userid"].Value = newUserId;
        output("inserting previousaccountinfo");
        infoCommand.ExecuteNonQuery();

        oldIds.Add(user.UserId, newUserId);
        newIds.Add(newUserId, user.UserId);
      }
    }

    private void UpdateNextHi(int id, MySqlConnection conn, MySqlTransaction tx) {
      output("updating next hi");

      var command = new MySqlCommand("select next_hi from hibernate_unique_key", conn, tx);
      int? next_hi = command.ExecuteScalar() as int?;
      var nextHi = next_hi.HasValue ? next_hi.Value : 1;

      var newNextHi = id / 1000 + 1;
      if (newNextHi > nextHi) {
        command = new MySqlCommand("update hibernate_unique_key set next_hi = " + newNextHi.ToString(), conn, tx);
        command.ExecuteNonQuery();
      }
    }

    private void button2_Click(object sender, EventArgs e) {
      try {
        beginTicks = DateTime.Now.Ticks;

        initializeNH();

        CreateSpecialUsersAndRoles();

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
    }

    private void CreateSpecialUsersAndRoles() {
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
