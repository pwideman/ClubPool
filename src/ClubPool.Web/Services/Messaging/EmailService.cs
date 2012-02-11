using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Configuration;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Infrastructure.Configuration;

namespace ClubPool.Web.Services.Messaging
{
  public class EmailService : IEmailService
  {
    protected string smtpHost;
    protected string systemEmailAddress;
    protected string systemEmailPassword;

    public EmailService(ClubPoolConfiguration config) {
      Arg.NotNull(config, "config");

      smtpHost = config.SmtpHost;
      systemEmailAddress = config.SystemEmailAddress;
      systemEmailPassword = config.SystemEmailPassword;
    }

    public void SendSystemEmail(string to, string subject, string body) {
      SendSystemEmail(new List<string>() { to }, subject, body);
    }

    public void SendSystemEmail(IList<string> to, string subject, string body) {
      SendSystemEmail(to, null, null, subject, body);
    }

    public void SendSystemEmail(IList<string> to, IList<string> cc, IList<string> bcc, string subject, string body) {
      body += Environment.NewLine + Environment.NewLine + "This is a post only email address. Do not reply to this email, it is not monitored or read.";
      SendEmail(SystemEmailAddress, to, cc, bcc, subject, body);
    }

    public void SendEmail(string from, string to, string subject, string body) {
      SendEmail(from, new List<string>() { to }, subject, body);
    }

    public void SendEmail(string from, IList<string> to, string subject, string body) {
      SendEmail(from, to, null, null, subject, body);
    }

    public void SendEmail(string from, IList<String> to, IList<string> cc, IList<string> bcc, string subject, string body) {
      Arg.NotNull(from, "from");
      Arg.Require(null != to && to.Count > 0, "to cannot be null or zero length");

      using(var mm = new MailMessage()) {
        mm.From = new MailAddress(SystemEmailAddress);
        AddAddressesToCollection(to, mm.To);
        AddAddressesToCollection(cc, mm.CC);
        AddAddressesToCollection(bcc, mm.Bcc);
        mm.ReplyTo = new MailAddress(from);
        mm.Subject = subject;
        mm.Body = body;

        var client = GetSmtpClient();
        client.Send(mm);
      }
    }

    protected void AddAddressesToCollection(IList<string> recipients, MailAddressCollection collection) {
      if (null != recipients && recipients.Count > 0) {
        foreach (var recipient in recipients) {
          if (!string.IsNullOrEmpty(recipient)) {
            collection.Add(new MailAddress(recipient));
          }
        }
      }
    }

    protected virtual SmtpClient GetSmtpClient() {
      var client = new SmtpClient(smtpHost);
      client.EnableSsl = false;
      if (!string.IsNullOrEmpty(systemEmailPassword)) {
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(systemEmailAddress, systemEmailPassword);
      }
      return client;
    }

    protected virtual string SystemEmailAddress {
      get { return systemEmailAddress; }
    }
  }
}