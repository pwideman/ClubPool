using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Framework.Configuration
{
  public class ClubPoolConfiguration
  {
    public ClubPoolConfiguration() {
    }

    public ClubPoolConfiguration(string siteName, string smtpHost, string systemEmailAddress, bool useRescues) {
      SiteName = siteName;
      SmtpHost = smtpHost;
      SystemEmailAddress = systemEmailAddress;
      UseRescues = useRescues;
    }

    public string SiteName { get; protected set; }
    public bool UseRescues { get; protected set; }
    public string SmtpHost { get; protected set; }
    public string SystemEmailAddress { get; protected set; }
  }
}
