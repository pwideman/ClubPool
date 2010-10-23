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

using ClubPool.Core;
using ClubPool.Data;
using ClubPool.Data.NHibernateMaps;
using ClubPool.ApplicationServices.Membership;
using ClubPool.Framework.NHibernate;

namespace ClubPool.SchemaGen
{
  public partial class SchemaGen : Form
  {
    public SchemaGen() {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      try {
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
        Role admin = null;
        Role officer = null;
        using (roleRepo.DbContext.BeginTransaction()) {
          admin = new Role(Roles.Administrators);
          roleRepo.SaveOrUpdate(admin);
          officer = new Role(Roles.Officers);
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

        output("Creating dummy data");
        var seasonRepo = new SeasonRepository();
        int userIndex = 1;
        var users = new List<User>();
        using (userRepo.DbContext.BeginTransaction()) {
          for (userIndex = 1; userIndex <= 60; userIndex++) {
            var username = "user " + userIndex.ToString();
            users.Add(membershipService.CreateUser(username, "user", "user", userIndex.ToString(), 
              "user" + userIndex.ToString() + "@email.com", true, false));
          }
          userRepo.DbContext.CommitTransaction();
        }

        using (seasonRepo.DbContext.BeginTransaction()) {
          for (int seasonIndex = 1; seasonIndex <= 5; seasonIndex++) {
            output("Creating season " + seasonIndex.ToString());
            var season = new Season("Season " + seasonIndex.ToString());
            season.IsActive = false;
            userIndex = 0;
            for (int divisionIndex = 1; divisionIndex <= 2; divisionIndex++) {
              output("Creating division " + divisionIndex.ToString());
              var division = new Division("Division " + divisionIndex.ToString(), DateTime.Parse("1/" + divisionIndex.ToString() + "/200" + seasonIndex.ToString()), season);
              season.AddDivision(division);
              for (int teamIndex = 1; teamIndex <= 12; teamIndex++) {
                output("Creating team " + teamIndex.ToString());
                var team = new Team("Team " + teamIndex.ToString(), division);
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
      OutputTextBox.Text += Environment.NewLine + text;
      OutputTextBox.Update();
    }
  }
}
