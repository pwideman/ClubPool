using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Web.Infrastructure.Configuration;

namespace ClubPool.Web.Services.Configuration
{
  public class ConfigurationService : IConfigurationService
  {
    public ClubPoolConfiguration GetConfig() {
      return ClubPoolConfigurationSection.GetConfig();
    }
  }
}
