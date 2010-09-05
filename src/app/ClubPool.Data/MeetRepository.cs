using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core.Contracts;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;

namespace ClubPool.Data
{
  public class MeetRepository : LinqRepository<Meet>, IMeetRepository
  {
  }
}
