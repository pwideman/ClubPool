using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core;

using ClubPool.ApplicationServices.DomainManagement.Contracts;
using ClubPool.Core;
using ClubPool.Framework.Extensions;
using ClubPool.Core.Contracts;

namespace ClubPool.ApplicationServices.DomainManagement
{
  public class DivisionManagementService : IDivisionManagementService
  {
    protected IDivisionRepository divisionRepository;
    protected IMeetRepository meetRepository;

    public DivisionManagementService(IDivisionRepository divisionRepository, IMeetRepository meetRepository) {
      Check.Require(null != divisionRepository, "divisionRepository cannot be null");
      Check.Require(null != meetRepository, "meetRepository cannot be null");

      this.divisionRepository = divisionRepository;
      this.meetRepository = meetRepository;
    }

    public void CreateSchedule(Division division) {
      var teams = new List<Team>();
      division.Teams.Each(t => teams.Add(t));
      var numTeams = teams.Count;
      if (numTeams < 2) {
        throw new ArgumentException("division must have 2 or more teams to create a schedule", "division");
      }

      var includeBye = (numTeams % 2 != 0) ? true : false;
      var numMatches = numTeams / 2;
      var numWeeks = includeBye ? numTeams : numTeams - 1;
      var opponent = -1;
      var meets = new List<Meet>();

      using (meetRepository.DbContext.BeginTransaction()) {
        for (int i = 0; i < numWeeks; i++) {
          for (int j = 0; j < numTeams; j++) {
            if (includeBye) {
              opponent = (numTeams + i - j - 1) % numTeams;
            }
            else {
              if (j < (numTeams - 1)) {
                if (i == ((2 * j + 1) % (numTeams - 1))) {
                  opponent = numTeams - 1;
                }
                else {
                  opponent = ((numTeams - 1) + i - j - 1) % (numTeams - 1);
                }
              }
              else {
                for (int p = 0; p < numTeams; p++) {
                  if (i == (2 * p + 1) % (numTeams - 1)) {
                    opponent = p;
                    break;
                  }
                }
              }
            }
            if (opponent != j) {
              if (!meets.Where(m => m.Teams.Contains(teams[j]) && m.Teams.Contains(teams[opponent])).Any()) {
                Meet m = new Meet(teams[j], teams[opponent], i);
                meetRepository.SaveOrUpdate(m);
                meets.Add(m);
              }
            }
          }
        }
        divisionRepository.Refresh(division);
        meetRepository.DbContext.CommitTransaction();
      }
    }

    public bool DivisionNameIsInUse(Season season, string name) {
      return divisionRepository.GetAll().Where(d => d.Season == season && d.Name == name).Any();
    }
  }
}
