using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.NHibernate;

namespace ClubPool.Core.Contracts
{
  public interface IDivisionRepository : ILinqRepository<Division>
  {
  }
}
