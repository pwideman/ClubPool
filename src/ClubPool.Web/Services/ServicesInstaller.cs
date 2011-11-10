using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

namespace ClubPool.Web.Services
{
  public class ServicesInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store) {
      container.Register(
          AllTypes.FromThisAssembly()
          .Pick().WithService.DefaultInterface().Configure(c => c.LifeStyle.Transient));
    }
  }
}