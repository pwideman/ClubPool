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

using ClubPool.Data.NHibernateMaps;
using SharpArch.Data.NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace ClubPool.SchemaGen
{
  public partial class SchemaGen : Form
  {
    public SchemaGen() {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      try {
        var mappingAssemblies = new string[] { "ClubPool.Data" };
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
