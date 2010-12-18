using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ClubPool.Framework.Configuration
{
  public class ClubPoolConfigurationSection : ConfigurationSection
  {
    public static ClubPoolConfigurationSection GetConfig() {
      return ConfigurationManager.GetSection("clubPool") as ClubPoolConfigurationSection;
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