using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core;

using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.ApplicationServices.DomainManagement.Contracts;

namespace ClubPool.ApplicationServices.DomainManagement
{
  public class TeamManagementService : ITeamManagementService
  {
    protected ITeamRepository teamRepository;

    public TeamManagementService(ITeamRepository teamRepo) {
      Check.Require(null != teamRepo, "teamRepo cannot be null");

      teamRepository = teamRepo;
    }

    public bool TeamNameIsInUse(Division division, string name) {
      return teamRepository.GetAll().Where(t => t.Division == division && t.Name == name).Any();
    }
  }
}
