using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

using SharpArch.Web.Castle;

namespace ClubPool.Web.CastleWindsor
{
  public class CustomRepositoriesInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store) {
      container.Register(
        AllTypes.FromAssemblyNamed("ClubPool.Data").Pick()
        .WithService.FirstNonGenericCoreInterface("ClubPool.Core"));

    }
  }
}