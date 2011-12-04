using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;

namespace ClubPool.Data
{
  public class UserRepository : LinqRepository<User>, IUserRepository
  {
    public IList<User> GetUnassignedUsersForSeason(Season season) {
      var query = Session.GetNamedQuery("GetUnassignedPlayersForSeason")
        .SetInt32("SeasonId", season.Id);

      return query.List<User>();
    }
  }
}
