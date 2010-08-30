using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

namespace ClubPool.Web.CastleWindsor
{
  public class ApplicationServicesInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store) {
      container.Register(
          AllTypes.FromAssemblyNamed("ClubPool.ApplicationServices")
          .Pick().WithService.DefaultInterface());
    }
  }
}