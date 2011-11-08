using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;

namespace ClubPool.Data
{
  public class TeamRepository : LinqRepository<Team>, ITeamRepository
  {
  }
}
