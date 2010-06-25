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
        var roleRepo = new LinqRepository<Role>();
        Role admin = null;
        Role officer = null;
        using (roleRepo.DbContext.BeginTransaction()) {
          admin = new Role(Roles.Administrators);
          roleRepo.SaveOrUpdate(admin);
          officer = new Role(Roles.Officers);
          roleRepo.SaveOrUpdate(officer);
          roleRepo.DbContext.CommitTransaction();
        }

        output("Creating users");
        var userRepo = new LinqRepository<User>();
        using (userRepo.DbContext.BeginTransaction()) {
          var membershipService = new SharpArchMembershipService(userRepo);
          // create admin user
          var user = membershipService.CreateUser("admin", "admin", "admin", "user", "admin@admin.com", true, false);
          user.AddRole(admin);
          userRepo.SaveOrUpdate(user);
          // create officer user
          user = membershipService.CreateUser("officer", "officer", "officer", "user", "officer@email.com", true, false);
          user.AddRole(officer);
          userRepo.SaveOrUpdate(user);
          // create normal user
          membershipService.CreateUser("user", "user", "normal", "user", "user@user.com", true, false);
          // create some dummy unapproved users
          for (int i = 0; i < 10; i++) {
            var name = "user" + i.ToString();
            membershipService.CreateUser(name, name, "user", i.ToString(), name + "@email.com", false, false);
          }
          // create some dummy locked users
          for (int i = 10; i < 20; i++) {
            var name = "user" + i.ToString();
            membershipService.CreateUser(name, name, "user", i.ToString(), name + "@email.com", true, true);
          }
          userRepo.DbContext.CommitTransaction();
        }

        output("Creating seasons");
        var seasonRepo = new LinqRepository<Season>();
        using (seasonRepo.DbContext.BeginTransaction()) {
          for (int i = 0; i < 150; i++) {
            var s = new Season("Season " + i.ToString());
            seasonRepo.SaveOrUpdate(s);
          }
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
