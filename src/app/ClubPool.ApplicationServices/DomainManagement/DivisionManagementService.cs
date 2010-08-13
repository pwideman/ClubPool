using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core;

using ClubPool.ApplicationServices.DomainManagement.Contracts;
using ClubPool.Core;
using ClubPool.Core.Contracts;

namespace ClubPool.ApplicationServices.DomainManagement
{
  public class DivisionManagementService : IDivisionManagementService
  {
    protected IDivisionRepository divisionRepository;

    public DivisionManagementService(IDivisionRepository divisionRepository) {
      Check.Require(null != divisionRepository, "divisionRepository cannot be null");

      this.divisionRepository = divisionRepository;
    }

    public bool DivisionNameIsInUse(Season season, string name) {
      return divisionRepository.GetAll().Where(d => d.Season == season && d.Name == name).Any();
    }
  }
}
