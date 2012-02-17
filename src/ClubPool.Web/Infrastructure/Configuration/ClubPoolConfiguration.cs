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

    public string SiteName { get; set; }
    public bool UseRescues { get; set; }
    public string SmtpHost { get; set; }
    public string SystemEmailAddress { get; set; }
    public string SystemEmailPassword { get; set; }
    public string AppRootPath { get; set; }
    public string AppVersion { get; set; }
  }
}
