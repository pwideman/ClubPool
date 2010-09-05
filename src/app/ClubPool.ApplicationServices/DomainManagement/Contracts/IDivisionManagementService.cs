using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.ApplicationServices.DomainManagement.Contracts
{
  public interface IDivisionManagementService
  {
    bool DivisionNameIsInUse(Season season, string name);
    void CreateSchedule(Division division);
  }
}
