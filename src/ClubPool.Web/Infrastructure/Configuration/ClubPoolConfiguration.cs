using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Infrastructure.Configuration
{
  public class ClubPoolConfiguration
  {
    public ClubPoolConfiguration() {
    }

    public ClubPoolConfiguration(string siteName, string smtpHost, string systemEmailAddress, string systemEmailPassword, bool useRescues) {
      SiteName = siteName;
      SmtpHost = smtpHost;
      SystemEmailAddress = systemEmailAddress;
      SystemEmailPassword = systemEmailPassword;
      UseRescues = useRescues;
    }

    public string SiteName { get; protected set; }
    public bool UseRescues { get; protected set; }
    public string SmtpHost { get; protected set; }
    public string SystemEmailAddress { get; protected set; }
    public string SystemEmailPassword { get; protected set; }
  }
}
