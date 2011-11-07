using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.Configuration;

namespace ClubPool.Web.Services.Configuration
{
  public interface IConfigurationService
  {
    ClubPoolConfiguration GetConfig();
  }
}
