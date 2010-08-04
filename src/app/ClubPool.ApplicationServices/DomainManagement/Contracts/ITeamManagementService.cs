using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.ApplicationServices.DomainManagement.Contracts
{
  public interface ITeamManagementService
  {
    bool TeamNameIsInUse(Division division, string name);
  }
}
