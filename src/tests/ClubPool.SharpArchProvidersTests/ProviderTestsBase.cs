using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using SharpArch.Testing.NHibernate;
using SharpArch.Data.NHibernate;

namespace Tests.ClubPool.SharpArchProviders
{
  public abstract class ProviderTestsBase
  {
    [SetUp]
    protected virtual void SetUp() {
      ServiceLocatorInitializer.Init();
      RepositoryTestsHelper.InitializeDatabase();
      InitializeProvider();
      LoadTestData();
      FlushAndClearSession();
    }

    [TearDown]
    public virtual void TearDown() {
      ShutdownDatabase();
    }

    protected virtual void ShutdownDatabase() {
      RepositoryTestsHelper.Shutdown();
    }

    protected virtual void FlushSessionAndEvict(object instance) {
      RepositoryTestsHelper.FlushSessionAndEvict(instance);
    }

    protected virtual void FlushAndClearSession() {
      NHibernateSession.Current.Flush();
      NHibernateSession.Current.Clear();
    }

    protected abstract void LoadTestData();

    protected abstract void InitializeProvider();

  }
}
