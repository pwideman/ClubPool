using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

using SharpArch.Core.CommonValidator;
using SharpArch.Core.NHibernateValidator.CommonValidatorAdapter;

namespace ClubPool.Web.CastleWindsor
{
  public class ValidatorInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store) {
      container.Register(Component.For(typeof(IValidator)).ImplementedBy(typeof(Validator)));
    }
  }
}