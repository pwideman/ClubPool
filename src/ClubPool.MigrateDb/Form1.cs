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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Transactions;

using SharpArch.Data.NHibernate;
using MySql.Data.MySqlClient;
using NHibernate.Tool.hbm2ddl;
using nhconfig=NHibernate.Cfg;
using log4net;
using NHibernate.Linq;
using NHibernate;

using Core=ClubPool.Core;
using ClubPool.Data;
using ClubPool.Data.NHibernateMaps;
using ClubPool.Web.Services.Membership;
using ClubPool.Framework.NHibernate;

using ClubPool.Web.Infrastructure.EntityFramework;
using Models=ClubPool.Web.Models;

namespace ClubPool.MigrateDb
{
  public partial class MigrateDb : Form
  {
    protected long beginTicks = 0;
    protected nhconfig.Configuration nhConfig;
    protected Action workerAction = null;
    protected static readonly ILog logger = LogManager.GetLogger(typeof(MigrateDb));

    public MigrateDb() {
      //Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
      Database.SetInitializer<ClubPoolContext>(new DropCreateDatabaseAlways<ClubPoolContext>());
      InitializeComponent();
    }

    private void importIPDataSQLButton_Click(object sender, EventArgs e) {
      startBackgroundWorker(() => ImportIPData());
    }

    private void testButton_Click(object sender, EventArgs e) {
      startBackgroundWorker(() => Test());
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
      importIPDataSQLButton.Enabled = enable;
    }

    private void ProcessDBAction(Action action) {
      try {
        beginTicks = DateTime.Now.Ticks;
        EnsureNHConfig();

        using (new CursorKeeper(Cursors.WaitCursor)) {
          action();
        }
      }
      catch (System.Data.Entity.Validation.DbEntityValidationException vex) {
        output("Validation exception: ");
        output(getExceptionText(vex));
        foreach(var error in vex.EntityValidationErrors) {
          foreach(var err in error.ValidationErrors) {
            output(string.Format("Validation error for property {0}: {1}", err.PropertyName, err.ErrorMessage));
          }
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
        text.Append(Environment.NewLine + getExceptionText(e.InnerException));
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

    private void Test() {
      output("Opening EF connection");
      using (var context = new ClubPoolContext()) {
        output("Starting transaction");
        context.BeginTransaction();
        var r = new Models.Role("TestRole");
        context.Roles.Add(r);
        context.SaveChanges();
      }
    }

    private void ImportIPData() {
      output("Opening NH connection");
      using (var session = NHibernateSession.GetDefaultSessionFactory().OpenSession()) {
        output("Opening EF connection");
        using (var context = new ClubPoolContext()) {
          ImportRoles(session, context);
          ImportUsers(session, context);
          ImportSeasons(session, context);
          output("Saving changes");
          context.SaveChanges();
        }
      }
      output("Finished");
    }

    private void ImportRoles(ISession session, ClubPoolContext context) {
      output("Importing roles");
      foreach (var role in session.Query<Core.Role>()) {
        var newrole = new Models.Role(role.Name);
        context.Roles.Add(newrole);
      }
    }

    private void ImportUsers(ISession session, ClubPoolContext context) {
      output("Importing users");
      foreach (var user in session.Query<Core.User>()) {
        // some emails may be missing
        var email = string.IsNullOrWhiteSpace(user.Email) ? "unknown@example.com" : user.Email;
        var newuser = new Models.User(user.Username, user.Password, user.FirstName, user.LastName, email);
        newuser.PasswordSalt = user.PasswordSalt;
        newuser.IsApproved = user.IsApproved;
        newuser.IsLocked = user.IsLocked;
        foreach (var role in user.Roles) {
          newuser.Roles.Add(context.Roles.Local.Single(r => r.Name == role.Name));
        }
        ImportSkillLevels(user, newuser, context);
        context.Users.Add(newuser);
      }
    }

    private void ImportSkillLevels(Core.User oldUser, Models.User newUser, ClubPoolContext context) {
      output("Importing skill levels for user " + newUser.Username);
      foreach (var sl in oldUser.SkillLevels) {
        var newsl = new Models.SkillLevel(newUser, ClubPool.Web.Infrastructure.GameType.EightBall, sl.Value);
        newUser.SkillLevels.Add(newsl);
      }
    }

    private void ImportSeasons(ISession session, ClubPoolContext context) {
      output("Importing seasons");
      foreach (var season in session.Query<Core.Season>()) {
        var newseason = new Models.Season(season.Name, ClubPool.Web.Infrastructure.GameType.EightBall) {
          IsActive = season.IsActive
        };
        context.Seasons.Add(newseason);
        ImportDivisions(season, newseason, session, context);
      }
    }

    private void ImportDivisions(Core.Season oldSeason, Models.Season newSeason, ISession session, ClubPoolContext context) {
      output("Importing divisions for season " + newSeason.Name);
      foreach (var division in oldSeason.Divisions) {
        var newdivision = new Models.Division(division.Name, division.StartingDate, newSeason);
        newSeason.Divisions.Add(newdivision);
        context.Divisions.Add(newdivision);
        ImportTeams(division, newdivision, context);
        ImportMeets(division, newdivision, context);
      }
    }

    private void ImportMeets(Core.Division oldDivision, Models.Division newDivision, ClubPoolContext context) {
      output("Importing meets for division " + newDivision.Name);
      foreach (var meet in oldDivision.Meets) {
        var oldTeam1 = meet.Teams.ElementAt(0);
        var oldTeam2 = meet.Teams.ElementAt(1);
        var team1 = newDivision.Teams.Single(t => t.Name == oldTeam1.Name);
        var team2 = newDivision.Teams.Single(t => t.Name == oldTeam2.Name);
        var newmeet = new Models.Meet(team1, team2, meet.Week) {
          IsComplete = meet.IsComplete
        };
        context.Meets.Add(newmeet);
        newDivision.Meets.Add(newmeet);
        ImportMatches(meet, newmeet, context);
      }
    }

    private void ImportMatches(Core.Meet oldMeet, Models.Meet newMeet, ClubPoolContext context) {
      output("Importing matches for meet " + oldMeet.Id);
      foreach (var match in oldMeet.Matches) {
        var oldMatchPlayer1 = match.Players.ElementAt(0);
        var oldMatchPlayer2 = match.Players.ElementAt(1);
        var newmatch = new Models.Match(newMeet,
          new Models.MatchPlayer(context.Users.Local.Single(u => u.Username == oldMatchPlayer1.Player.Username),
            newMeet.Division.Teams.Single(t => t.Name == oldMatchPlayer1.Team.Name)),
          new Models.MatchPlayer(context.Users.Local.Single(u => u.Username == oldMatchPlayer2.Player.Username),
            newMeet.Division.Teams.Single(t => t.Name == oldMatchPlayer2.Team.Name)));
        newmatch.IsForfeit = match.IsForfeit;
        if (newmatch.IsForfeit) {
          newmatch.DatePlayed = null;
        }
        else {
          newmatch.DatePlayed = match.DatePlayed;
        }
        newmatch.IsComplete = match.IsComplete;
        if (newmatch.IsComplete) {
          newmatch.Winner = context.Users.Local.Single(u => u.Username == match.Winner.Username);
        }
        newMeet.Matches.Add(newmatch);
        context.Matches.Add(newmatch);
        ImportMatchResults(match, newmatch, context);
      }
    }

    private void ImportMatchResults(Core.Match oldMatch, Models.Match newMatch, ClubPoolContext context) {
      output("Importing match results for match " + oldMatch.Id);
      foreach (var result in oldMatch.Results) {
        var newresult = new Models.MatchResult(context.Users.Local.Single(u => u.Username == result.Player.Username),
          result.Innings, result.DefensiveShots, result.Wins) {
          Match = newMatch
        };
        newMatch.Results.Add(newresult);
        context.MatchResults.Add(newresult);
      }
    }

    private void ImportTeams(Core.Division oldDivision, Models.Division newDivision, ClubPoolContext context) {
      output("Importing teams for division " + newDivision.Name);
      foreach (var team in oldDivision.Teams) {
        var newteam = new Models.Team(team.Name, newDivision) {
          SchedulePriority = team.SchedulePriority
        };
        foreach (var player in team.Players) {
          newteam.Players.Add(context.Users.Local.Single(u => u.Username == player.Username));
        }
        newDivision.Teams.Add(newteam);
        context.Teams.Add(newteam);
      }
    }

    private void initializeNH() {
      var mappingAssemblies = new string[] { "ClubPool.Data.dll" };
      output("Creating NH configuration...");
      nhConfig = NHibernateSession.Init(new SimpleSessionStorage(), mappingAssemblies,
                    new AutoPersistenceModelGenerator().Generate(),
                    "NHibernate.config");
    }

    protected void EnsureNHConfig() {
      if (null == nhConfig) {
        initializeNH();
      }
    }


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
