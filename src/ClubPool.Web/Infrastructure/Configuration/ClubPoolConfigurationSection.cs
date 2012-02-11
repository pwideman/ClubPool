using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ClubPool.Web.Infrastructure.Configuration
{
  public class ClubPoolConfigurationSection : ConfigurationSection
  {
    public static ClubPoolConfiguration GetConfig() {
      var section = ConfigurationManager.GetSection("clubPool") as ClubPoolConfigurationSection;
      return new ClubPoolConfiguration {
        SiteName = section.SiteName,
        SmtpHost = section.SmtpHost,
        SystemEmailAddress = section.SystemEmailAddress,
        SystemEmailPassword = section.SystemEmailPassword,
        UseRescues = section.UseRescues
      };
    }

    [ConfigurationProperty("smtpHost")]
    public string SmtpHost {
      get {
        return this["smtpHost"] as string;
      }
    }

    [ConfigurationProperty("systemEmailAddress")]
    public string SystemEmailAddress {
      get {
        return this["systemEmailAddress"] as string;
      }
    }

    [ConfigurationProperty("systemEmailPassword")]
    public string SystemEmailPassword {
      get {
        return this["systemEmailPassword"] as string;
      }
    }

    [ConfigurationProperty("useRescues", DefaultValue=false)]
    public bool UseRescues {
      get {
        return (bool)this["useRescues"];
      }
    }

    [ConfigurationProperty("siteName", DefaultValue="ClubPool")]
    public string SiteName {
      get {
        return this["siteName"] as string;
      }
    }
  }
}