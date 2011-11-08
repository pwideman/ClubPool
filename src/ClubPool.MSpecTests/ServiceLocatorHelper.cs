// this class is based on the ServiceLocatorHelper in WhoCanHelpMe (http://whocanhelpme.codeplex.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.ServiceLocation;
using Rhino.Mocks;
using SharpArch.Core.CommonValidator;
using SharpArch.Core.NHibernateValidator.CommonValidatorAdapter;

namespace ClubPool.MSpecTests
{
  public static class ServiceLocatorHelper
  {
    private static IServiceLocator provider;

    public static void InitialiseServiceLocator() {
      provider = MockRepository.GenerateStub<IServiceLocator>();

      ServiceLocator.SetLocatorProvider(() => provider);
    }

    public static IValidator AddValidator() {
      if (provider == null) {
        InitialiseServiceLocator();
      }

      var validator = new Validator();
      validator.AddToServiceLocator<IValidator>();
      return validator;
    }

    public static T AddToServiceLocator<T>(this T o) {
      if (provider == null) {
        InitialiseServiceLocator();
      }

      provider.Stub(p => p.GetInstance<T>()).Return(o);
      provider.Stub(p => p.GetInstance(typeof(T))).Return(o);
      provider.Stub(p => p.GetService(typeof(T))).Return(o);
      return o;
    }
  }
}
