using System;

namespace ClubPool.Web.Infrastructure
{
  public interface IDbContext
  {
    int SaveChanges();
    void CommitTransaction();
    void RollbackTransaction();
    IDisposable BeginTransaction();
  }
}
