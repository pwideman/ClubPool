using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Services.Messaging
{
  public interface IEmailService
  {
    void SendEmail(string from, string to, string subject, string body);
    void SendEmail(string from, IList<string> to, string subject, string body);
    void SendEmail(string from, IList<string> to, IList<string> cc, IList<string> bcc, string subject, string body);

    void SendSystemEmail(string to, string subject, string body);
    void SendSystemEmail(IList<string> to, string subject, string body);
    void SendSystemEmail(IList<string> to, IList<string> cc, IList<string> bcc, string subject, string body);
  }
}
