using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.Configuration;
using ClubPool.ApplicationServices.Configuration.Contracts;

namespace ClubPool.ApplicationServices.Configuration
{
  public class ConfigurationService : IConfigurationService
  {
    public ClubPoolConfiguration GetConfig() {
      return ClubPoolConfigurationSection.GetConfig();
    }
  }
}
