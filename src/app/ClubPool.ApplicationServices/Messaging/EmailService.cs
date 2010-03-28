using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

using SharpArch.Core;

using ClubPool.ApplicationServices.Messaging.Contracts;

namespace ClubPool.ApplicationServices.Messaging
{
  public class EmailService : IEmailService
  {
    protected string smtpHost;
    protected string systemEmailAddress;

    public EmailService() {
      smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
      systemEmailAddress = ConfigurationManager.AppSettings["SystemEmailAddress"];
    }

    public void SendSystemEmail(string to, string subject, string body) {
      SendEmail(SystemEmailAddress, to, subject, body);
    }

    public void SendSystemEmail(IList<string> to, string subject, string body) {
      SendEmail(SystemEmailAddress, to, subject, body);
    }

    public void SendSystemEmail(IList<string> to, IList<string> cc, IList<string> bcc, string subject, string body) {
      SendEmail(SystemEmailAddress, to, cc, bcc, subject, body);
    }

    public void SendEmail(string from, string to, string subject, string body) {
      SendEmail(from, new List<string>() { to }, subject, body);
    }

    public void SendEmail(string from, IList<string> to, string subject, string body) {
      SendEmail(from, to, null, null, subject, body);
    }

    public void SendEmail(string from, IList<String> to, IList<string> cc, IList<string> bcc, string subject, string body) {
      Check.Require(!string.IsNullOrEmpty(from), "from cannot be null or empty");
      Check.Require(null != to && to.Count > 0, "to cannot be null or zero length");

      MailMessage mm = new MailMessage();
      
      mm.From = new MailAddress(from);
      AddAddressesToCollection(to, mm.To);
      AddAddressesToCollection(cc, mm.CC);
      AddAddressesToCollection(bcc, mm.Bcc);

      mm.Subject = subject;
      mm.Body = body;

      var client = SmtpClient;
      if (!string.IsNullOrEmpty(smtpHost)) {
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

    protected virtual SmtpClient SmtpClient {
      get {
        var client = new SmtpClient(smtpHost);
        client.EnableSsl = false;
        client.UseDefaultCredentials = false;
        return client;
      }
    }

    protected virtual string SystemEmailAddress {
      get { return systemEmailAddress; }
    }
  }
}