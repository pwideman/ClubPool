﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Mvc;

using MvcContrib;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Core;

namespace ClubPool.Web.CastleWindsor
{
  /// <summary>
  /// Copied from MvcContrib.Castle. MvcContrib.Castle is not compatible with Castle 2.5,
  /// so we must remove all references to it
  /// </summary>
  public static class WindsorExtensions
  {
    public static IWindsorContainer RegisterControllersByTypes(this IWindsorContainer container, params Type[] controllerTypes) {
      foreach (var type in controllerTypes) {
        if (ControllerExtensions.IsController(type)) {
          container.Register(Component.For(type).Named(type.FullName.ToLower()).LifeStyle.Is(LifestyleType.Transient));
        }
      }

      return container;
    }

    public static IWindsorContainer RegisterControllersByAssemblies(this IWindsorContainer container, params Assembly[] assemblies) {
      foreach (var assembly in assemblies) {
        container.RegisterControllersByTypes(assembly.GetExportedTypes());
      }
      return container;
    }
  }
}