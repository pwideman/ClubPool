using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace ClubPool.Testing
{
  public abstract class SpecificationContext
  {
    [TestFixtureSetUp]
    public void Init() {
      Given();
      When();
    }

    public virtual void Given() { }
    public virtual void When() { }
  }
}
