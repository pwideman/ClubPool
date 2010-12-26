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
using System.Security.Cryptography;

using SharpArch.Data.NHibernate;
using NHibernate.Tool.hbm2ddl;
using MySql.Data.MySqlClient;
using nhconfig=NHibernate.Cfg;
using log4net;
using NHibernate.Linq;

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
    protected nhconfig.Configuration nhConfig;
    protected Action workerAction = null;
    protected static readonly ILog logger = LogManager.GetLogger(typeof(SchemaGen));

    public SchemaGen() {
      InitializeComponent();
    }

    private void createSchemaButton_Click(object sender, EventArgs e) {
      startBackgroundWorker(() => CreateSchema());
    }

    private void createSpecialUsersButton_Click(object sender, EventArgs e) {
      startBackgroundWorker(() => CreateSpecialUsersAndRoles());
    }

    private void createDummyDataButton_Click(object sender, EventArgs e) {
      startBackgroundWorker(() => CreateDummyData());
    }

    private void importIPDataSQLButton_Click(object sender, EventArgs e) {
      startBackgroundWorker(() => ImportIPDataSQL());
    }

    private void testButton_Click(object sender, EventArgs e) {
      startBackgroundWorker(() => TestProcess());
    }

    private void startBackgroundWorker(Action action) {
      logger.Info("Starting background worker");
      EnableButtons(false);
      backgroundWorker1.RunWorkerAsync(action);
    }

    private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      EnableButtons(true);
    }

    private void EnableButtons(bool enable) {
      createSchemaButton.Enabled = enable;
      createSpecialUsersButton.Enabled = enable;
      createDummyDataButton.Enabled = enable;
      importIPDataSQLButton.Enabled = enable;
      testButton.Enabled = enable;
    }

    private void ProcessDBAction(Action action) {
      try {
        beginTicks = DateTime.Now.Ticks;
        EnsureNHConfig();

        using (new CursorKeeper(Cursors.WaitCursor)) {
          action();
        }
      }
      catch (Exception ex) {
        output("Exception:");
        output(getExceptionText(ex));
      }
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
      var action = e.Argument as Action;
      ProcessDBAction(action);
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
      if (InvokeRequired) {
        Invoke(new Action(() => output(text)));
      }
      else {
        var elapsedTime = new TimeSpan(DateTime.Now.Ticks - beginTicks);
        var msg = Environment.NewLine + string.Format("{0}:{1}.{2} - {3}",
          elapsedTime.Minutes.ToString("00"),
          elapsedTime.Seconds.ToString("00"),
          elapsedTime.Milliseconds.ToString("000"), text);
        logger.Info(msg);
        OutputTextBox.AppendText(msg);
        OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
        OutputTextBox.ScrollToCaret();
        OutputTextBox.Update();
      }
    }

    private void TestProcess() {
      var seasonRepo = new SeasonRepository();
      //var season = seasonRepo.GetAll().Where(s => s.IsActive).FetchMany(s => s.Divisions).ThenFetchMany(d => d.Teams).ThenFetchMany(t => t.Players).Single();
      var season = seasonRepo.GetAll().Where(s => s.IsActive).Fetch(s => s.Divisions).Single();
      output("Active season: " + season.Name);
      foreach (var division in season.Divisions) {
        output("Division: " + division.Name);
        foreach (var team in division.Teams) {
          output("Team: " + team.Name);
          foreach (var player in team.Players) {
            output("Player: " + player.FullName);
          }
        }
        //foreach (var meet in division.Meets) {
        //  output("Meet: " + meet.Id);
        //  output(meet.Team1.Name + " vs " + meet.Team2.Name);
        //  foreach (var match in meet.Matches) {
        //    output("Match: " + match.Id);
        //    output(match.Player1.FullName + " vs " + match.Player2.FullName);
        //    foreach (var result in match.Results) {
        //      output("Result for " + result.Player.FullName);
        //    }
        //  }
        //}
      }
    }

    private void ImportIPDataSQL() {
      using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["cp"].ConnectionString)) {
        output("opening mysql connection");
        conn.Open();
        using (var tx = conn.BeginTransaction())
        using (var context = new ipoolEntities()) {
          var oldUserIdsToNewUserIds = new Dictionary<int, int>();
          var newUserIdsToOldUserIds = new Dictionary<int, int>();
          var oldUsers = new Dictionary<int, User>();

          var nextId = GetNextHi(conn, tx) * 1000 + 1;

          MigrateUsers(conn, tx, oldUserIdsToNewUserIds, newUserIdsToOldUserIds, context, oldUsers, ref nextId);

          MigrateSeasons(conn, tx, oldUserIdsToNewUserIds, newUserIdsToOldUserIds, context, ref nextId);

          UpdateNextHi(nextId, conn, tx);

          output("committing transaction");
          tx.Commit();

          var userRepo = new UserRepository();
          using (userRepo.DbContext.BeginTransaction()) {
            UpdateSKillLevels(userRepo, oldUsers);
            userRepo.DbContext.CommitTransaction();
          }

        }
        output("closing mysql connection");
        conn.Close();
        output("Finished");
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

      var oldTeamIds = new Dictionary<int, int>();

      foreach (var division in season.Divisions) {
        var divisionId = nextId++;
        cmd.Parameters["@id"].Value = divisionId;
        cmd.Parameters["@name"].Value = division.Description;
        cmd.Parameters["@date"].Value = season.StartDate.Value.AddDays(division.DateOffset);
        output(string.Format("inserting division '{0}'", division.Description));
        cmd.ExecuteNonQuery();
        MigrateTeams(division, divisionId, conn, tx, ref nextId, oldIds, newIds, oldTeamIds);
        MigrateMatches(division, divisionId, conn, tx, ref nextId, oldIds, newIds, oldTeamIds);
      }
    }

    private void MigrateMatches(Division division,
      int divisionId,
      MySqlConnection conn,
      MySqlTransaction tx,
      ref int nextId,
      Dictionary<int, int> oldIds,
      Dictionary<int, int> newIds,
      Dictionary<int, int> oldTeamIds) {

      var meetText = @"insert into meets (id, week, iscomplete, divisionid)
                       values (@id, @week, true, @divisionid);";
      var meetCmd = new MySqlCommand(meetText, conn, tx);
      meetCmd.Prepare();
      meetCmd.Parameters.AddWithValue("@id", 1);
      meetCmd.Parameters.AddWithValue("@week", 1);
      meetCmd.Parameters.AddWithValue("@divisionid", divisionId);

      var meetTeamText = @"insert into meetsteams (meetid, teamid) values (@meetid, @teamid);";
      var meetTeamCmd = new MySqlCommand(meetTeamText, conn, tx);
      meetTeamCmd.Prepare();
      meetTeamCmd.Parameters.AddWithValue("@meetid", 1);
      meetTeamCmd.Parameters.AddWithValue("@teamid", 1);

      var matchText = @"insert into matches (id, iscomplete, isforfeit, dateplayed, meetid, winnerid)
                        values (@id, true, @isforfeit, @dateplayed, @meetid, @winnerid);";
      var matchCmd = new MySqlCommand(matchText, conn, tx);
      matchCmd.Prepare();
      matchCmd.Parameters.AddWithValue("@id", 1);
      matchCmd.Parameters.AddWithValue("@isforfeit", false);
      matchCmd.Parameters.AddWithValue("@dateplayed", DateTime.Now);
      matchCmd.Parameters.AddWithValue("@meetid", 1);
      matchCmd.Parameters.AddWithValue("@winnerid", 1);

      var matchPlayerText = @"insert into matchesplayers (matchid, userid) values (@matchid, @playerid);";
      var matchPlayerCmd = new MySqlCommand(matchPlayerText, conn, tx);
      matchPlayerCmd.Prepare();
      matchPlayerCmd.Parameters.AddWithValue("@matchid", 1);
      matchPlayerCmd.Parameters.AddWithValue("@playerid", 1);

      var matchResultText = @"insert into matchresults (id, version, innings, defensiveshots, wins, matchid, playerid)
                              values (@id, 1, @innings, 0, @wins, @matchid, @playerid);";
      var resultCmd = new MySqlCommand(matchResultText, conn, tx);
      resultCmd.Prepare();
      resultCmd.Parameters.AddWithValue("@id", 1);
      resultCmd.Parameters.AddWithValue("@innings", 0);
      resultCmd.Parameters.AddWithValue("@wins", 0);
      resultCmd.Parameters.AddWithValue("@matchid", 0);
      resultCmd.Parameters.AddWithValue("@playerid", 0);

      var teamMeetsQuery = from m in division.Matches
                           group m by m.Team1 into team1Group
                           select new {
                             Team = team1Group.Key,
                             Meets =
                               from m in team1Group
                               group m by m.Team2 into team2Group
                               select new {
                                 Opponent = team2Group.Key,
                                 Matches = team2Group
                               }
                           };

      foreach (var team in teamMeetsQuery) {
        foreach (var meet in team.Meets) {
          var meetId = nextId++;
          meetCmd.Parameters["@id"].Value = meetId;
          meetCmd.Parameters["@week"].Value = meet.Matches.First().Week;
          output(string.Format("inserting meet for team1 {0} and team2 {1}", team.Team.Name, meet.Opponent.Name));
          meetCmd.ExecuteNonQuery();

          meetTeamCmd.Parameters["@meetid"].Value = meetId;
          meetTeamCmd.Parameters["@teamid"].Value = oldTeamIds[team.Team.ID];
          meetTeamCmd.ExecuteNonQuery();
          meetTeamCmd.Parameters["@teamid"].Value = oldTeamIds[meet.Opponent.ID];
          meetTeamCmd.ExecuteNonQuery();

          matchCmd.Parameters["@meetid"].Value = meetId;
          foreach (var match in meet.Matches) {
            var matchId = nextId++;
            matchCmd.Parameters["@id"].Value = matchId;
            matchCmd.Parameters["@isforfeit"].Value =
              (match.IsCompleted && match.Player1Innings == 0 && match.Player2Innings == 0 && match.Player1Wins == 0 && match.Player2Wins == 0);
            matchCmd.Parameters["@dateplayed"].Value = match.DatePlayed.Value;
            if (match.IsCompleted && match.WinnerId.HasValue && match.WinnerId.Value > 0) {
              matchCmd.Parameters["@winnerid"].Value = oldIds[match.WinnerId.Value];
            }
            else {
              matchCmd.Parameters["@winnerid"].Value = null;
            }
            output(string.Format("inserting match for player1 {0} and player2 {1}", match.Player1.UserName, match.Player2.UserName));
            matchCmd.ExecuteNonQuery();

            matchPlayerCmd.Parameters["@matchid"].Value = matchId;
            matchPlayerCmd.Parameters["@playerid"].Value = oldIds[match.Player1Id];
            matchPlayerCmd.ExecuteNonQuery();
            matchPlayerCmd.Parameters["@playerid"].Value = oldIds[match.Player2Id];
            matchPlayerCmd.ExecuteNonQuery();

            resultCmd.Parameters["@matchid"].Value = matchId;

            resultCmd.Parameters["@id"].Value = nextId++;
            resultCmd.Parameters["@innings"].Value = match.Player1Innings;
            resultCmd.Parameters["@wins"].Value = match.Player1Wins;
            resultCmd.Parameters["@playerid"].Value = oldIds[match.Player1Id];
            output(string.Format("inserting match results for player1 {0}", match.Player1.UserName));
            resultCmd.ExecuteNonQuery();

            resultCmd.Parameters["@id"].Value = nextId++;
            resultCmd.Parameters["@innings"].Value = match.Player2Innings;
            resultCmd.Parameters["@wins"].Value = match.Player2Wins;
            resultCmd.Parameters["@playerid"].Value = oldIds[match.Player2Id];
            output(string.Format("inserting match results for player2 {0}", match.Player2.UserName));
            resultCmd.ExecuteNonQuery();
          }
        }
      }
    }

    private void MigrateTeams(Division division,
      int divisionId,
      MySqlConnection conn,
      MySqlTransaction tx,
      ref int nextId,
      Dictionary<int, int> oldIds,
      Dictionary<int, int> newIds,
      Dictionary<int, int> oldTeamIds) {

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
        oldTeamIds.Add(team.ID, teamId);

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
      Dictionary<int, User> oldUsers,
      ref int nextId) {

      output("Migrating users");

      var commandText = @"insert into users(id, version, username, firstname, lastname, email, password, passwordsalt, isapproved, islocked) 
                          values (@id, 1, @username, @firstname, @lastname, @email, @password, @passwordsalt, true, false);";
      var command = new MySqlCommand(commandText, conn, tx);
      command.Prepare();
      command.Parameters.AddWithValue("@id", 1);
      command.Parameters.AddWithValue("@username", null);
      command.Parameters.AddWithValue("@firstname", null);
      command.Parameters.AddWithValue("@lastname", null);
      command.Parameters.AddWithValue("@email", null);
      command.Parameters.AddWithValue("@password", null);
      command.Parameters.AddWithValue("@passwordsalt", null);

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
        command.Parameters["@password"].Value = user.Password;
        command.Parameters["@passwordsalt"].Value = user.PasswordSalt;
        output("inserting user " + user.UserName);
        command.ExecuteNonQuery();

        oldIds.Add(user.UserId, newUserId);
        newIds.Add(newUserId, user.UserId);
        oldUsers.Add(newUserId, user);
      }
    }

    private int GetNextHi(MySqlConnection conn, MySqlTransaction tx) {
      var command = new MySqlCommand("select next_hi from hibernate_unique_key", conn, tx);
      int? next_hi = command.ExecuteScalar() as int?;
      return next_hi.HasValue ? next_hi.Value : 1;
    }

    private void UpdateNextHi(int id, MySqlConnection conn, MySqlTransaction tx) {
      output("updating next hi");

      var nextHi = GetNextHi(conn, tx);

      var newNextHi = id / 1000 + 1;
      if (newNextHi > nextHi) {
        var command = new MySqlCommand("update hibernate_unique_key set next_hi = " + newNextHi.ToString(), conn, tx);
        command.ExecuteNonQuery();
      }
    }

    private void UpdateSKillLevels(UserRepository userRepo, Dictionary<int, User> oldUsers) {
      var matchResultRepo = new MatchResultRepository();
      output("Updating skill levels");
      foreach (var user in userRepo.GetAll()) {
        output(string.Format("Calculating skill level for user '{0}'", user.Username));
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
          output(string.Format("***** WARNING: Different skill level for user '{0}', old: {1} new {2}", user.FullName, oldSL, newSL));
        }
      }
    }

    private void initializeNH() {
      var mappingAssemblies = new string[] { "ClubPool.Data.dll" };
      output("Creating NH configuration...");
      nhConfig = NHibernateSession.Init(new SimpleSessionStorage(), mappingAssemblies,
                    new AutoPersistenceModelGenerator().Generate(),
                    "NHibernate.config");
    }

    private void CreateSchema() {
      var sb = new StringBuilder();
      var sw = new StringWriter(sb);
      var session = NHibernateSession.GetDefaultSessionFactory().OpenSession();
      output("Exporting schema...");
      new SchemaExport(nhConfig).Execute(true, true, false, session.Connection, sw);

      sw.Flush();
      output("SchemaExport output:");
      output(sb.Replace("\n", Environment.NewLine).ToString());
      output("Finished");
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
        // create normal user
        user = membershipService.CreateUser("normal", "normal", "normal", "user", "normal@email.com", true, false);
        userRepo.SaveOrUpdate(user);

        userRepo.DbContext.CommitTransaction();
      }
      output("Finished");
    }

    protected void EnsureNHConfig() {
      if (null == nhConfig) {
        initializeNH();
      }
    }

    private void CreateDummyData() {
      var userRepo = new UserRepository();
      var membershipService = new SharpArchMembershipService(userRepo);

      output("Creating dummy data");
      int userIndex = 1;
      var users = new List<Core.User>();
      using (userRepo.DbContext.BeginTransaction()) {
        for (userIndex = 1; userIndex <= 60; userIndex++) {
          var username = "user" + userIndex.ToString();
          users.Add(membershipService.CreateUser(username, "user", "user", userIndex.ToString(),
            "user" + userIndex.ToString() + "@email.com", true, false));
        }
        userRepo.DbContext.CommitTransaction();
      }

      var seasonRepo = new SeasonRepository();
      var divisionRepo = new DivisionRepository();
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
            division.CreateSchedule(divisionRepo);
          }
          seasonRepo.SaveOrUpdate(season);
        }
        //output("Setting first season as active");
        //var firstSeason = seasonRepo.GetAll().First();
        //firstSeason.IsActive = true;
        //seasonRepo.SaveOrUpdate(firstSeason);
        output("Committing transaction");
        seasonRepo.DbContext.CommitTransaction();
      }
      output("Finished");
    }

    //private void ImportIPDataNH() {
    //  try {
    //    EnsureNHConfig();

    //    using (new CursorKeeper(Cursors.WaitCursor)) {
    //      beginTicks = DateTime.Now.Ticks;

    //      var userRepo = new UserRepository();
    //      var previousInfoRepo = new LinqRepository<Core.PreviousUserAccountInfo>();
    //      var membershipService = new SharpArchMembershipService(userRepo, true, true);
    //      var divisionRepo = new DivisionRepository();

    //      using (var context = new ipoolEntities()) {
    //        var oldUserIds = new Dictionary<int, Core.User>();
    //        var oldUsers = new Dictionary<int, User>();
    //        using (userRepo.DbContext.BeginTransaction()) {
    //          foreach (var user in context.Users) {
    //            var names = user.UserName.Split('_');
    //            var firstName = names[0].Substring(0, 1).ToUpper() + names[0].Substring(1);
    //            var lastName = "";
    //            if (names.Length > 1) {
    //              lastName = names[1].Substring(0, 1).ToUpper() + names[1].Substring(1);
    //            }
    //            var newUser = membershipService.CreateUser(user.UserName, user.PasswordSalt, firstName, lastName, user.Email, true, true);
    //            var previousInfo = new Core.PreviousUserAccountInfo(newUser, user.Password, user.PasswordSalt, user.UserId);
    //            previousInfoRepo.SaveOrUpdate(previousInfo);
    //            output(string.Format("Migrated user '{0}'", newUser.Username));
    //            oldUserIds.Add(user.UserId, newUser);
    //            oldUsers.Add(newUser.Id, user);
    //          }

    //          var seasonRepo = new SeasonRepository();
    //          foreach (var season in context.Seasons) {
    //            output(string.Format("Beginning migration for season '{0}'", season.Year.ToString()));

    //            var seasonName = "8-ball " + season.Year.ToString();
    //            var newSeason = new Core.Season(seasonName, Core.GameType.EightBall);
    //            seasonRepo.SaveOrUpdate(newSeason);
    //            output("Season created, migrating divisions");

    //            foreach (var division in season.Divisions) {
    //              if (division.Description != "HistoricalDummyDivision") {
    //                var newDivision = new Core.Division(division.Description, season.StartDate.Value.AddDays(division.DateOffset), newSeason);
    //                newSeason.AddDivision(newDivision);
    //                output(string.Format("Added division '{0}', migrating teams", newDivision.Name));

    //                var oldTeamIds = new Dictionary<Core.Team, int>();
    //                foreach (var team in division.Teams) {
    //                  var newTeam = new Core.Team(team.Name, newDivision);
    //                  newTeam.AddPlayer(oldUserIds[team.Player1ID]);
    //                  newTeam.AddPlayer(oldUserIds[team.Player2ID]);
    //                  newDivision.AddTeam(newTeam);
    //                  oldTeamIds.Add(newTeam, team.ID);
    //                  output(string.Format("Added team '{0}'", newTeam.Name));
    //                }

    //                output("Creating schedule");
    //                newDivision.CreateSchedule(divisionRepo);
    //                output("Migrating matches");
    //                foreach (var meet in newDivision.Meets) {
    //                  var team1Id = oldTeamIds[meet.Team1];
    //                  var team2Id = oldTeamIds[meet.Team2];
    //                  var matches = division.Matches.Where(m => (m.Team1Id == team1Id && m.Team2Id == team2Id) || (m.Team1Id == team2Id && m.Team2Id == team1Id));
    //                  int i = 0;
    //                  foreach (var match in matches) {
    //                    output(string.Format("Migrating old match '{0}'", match.ID.ToString()));
    //                    var player1 = oldUserIds[match.Player1Id];
    //                    var player2 = oldUserIds[match.Player2Id];
    //                    var newMatch = meet.Matches.ElementAt(i++);
    //                    newMatch.Player1 = player1;
    //                    newMatch.Player2 = player2;
    //                    if (match.Player1Wins == 0 && match.Player2Wins == 0) {
    //                      // forfeit
    //                      newMatch.IsForfeit = true;
    //                      output("Match is a forfeit");
    //                    }
    //                    else {
    //                      output("Adding results");
    //                      newMatch.AddResult(new Core.MatchResult(player1, match.Player1Innings, 0, match.Player1Wins));
    //                      newMatch.AddResult(new Core.MatchResult(player2, match.Player2Innings, 0, match.Player2Wins));
    //                      newMatch.DatePlayed = match.DatePlayed.Value;
    //                    }
    //                    newMatch.Winner = oldUserIds[match.WinnerId.Value];
    //                    newMatch.IsComplete = true;
    //                    output(string.Format("Finished match '{0}'", match.ID));
    //                  }
    //                }
    //                output(string.Format("Completed migration for division '{0}'", division.Description));
    //              }
    //            }
    //            output(string.Format("Completed migration for season '{0}'", newSeason.Name));
    //          }
    //          UpdateSKillLevels(userRepo, oldUsers);
    //          output("Committing transaction");
    //          userRepo.DbContext.CommitTransaction();
    //          output("Finished");
    //        }
    //      }
    //    }

    //  }
    //  catch (Exception ex) {
    //    output("Exception:");
    //    output(getExceptionText(ex));
    //  }
    //}
  }

  public class CursorKeeper : IDisposable
  {
    private Cursor _originalCursor;
    private bool _isDisposed = false;

    public CursorKeeper(Cursor newCursor) {
      _originalCursor = Cursor.Current;
      Cursor.Current = newCursor;
    }

    #region " IDisposable Support "
    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed) {
        if (disposing) {
          Cursor.Current = _originalCursor;
        }
      }
      _isDisposed = true;

    }

    public void Dispose() {
      // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion
  }

}
