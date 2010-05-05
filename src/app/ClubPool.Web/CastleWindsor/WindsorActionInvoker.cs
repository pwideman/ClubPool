using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

using Castle.Windsor;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;

namespace ClubPool.Web.CastleWindsor
{
  public class WindsorActionInvoker : ControllerActionInvoker
  {
    readonly IWindsorContainer container;

    public WindsorActionInvoker(IWindsorContainer container) {
      this.container = container;
    }

    //protected override ActionExecutedContext InvokeActionMethodWithFilters(
    //        ControllerContext controllerContext,
    //        IList<IActionFilter> filters,
    //        ActionDescriptor actionDescriptor,
    //        IDictionary<string, object> parameters) {
    //  foreach (IActionFilter actionFilter in filters) {
    //    container.Kernel.InjectProperties(actionFilter);
    //  }
    //  return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
    //}

    protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
      var filterInfo = base.GetFilters(controllerContext, actionDescriptor);

      foreach (var filter in filterInfo.ActionFilters) {
        container.Kernel.InjectProperties(filter);
      }
      foreach (var filter in filterInfo.AuthorizationFilters) {
        container.Kernel.InjectProperties(filter);
      }
      foreach (var filter in filterInfo.ExceptionFilters) {
        container.Kernel.InjectProperties(filter);
      }
      foreach (var filter in filterInfo.ResultFilters) {
        container.Kernel.InjectProperties(filter);
      }

      return filterInfo;
    }
  }

  public static class WindsorExtension
  {
    public static void InjectProperties(this IKernel kernel, object target) {
      var type = target.GetType();
      foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
        if (property.CanWrite && kernel.HasComponent(property.PropertyType)) {
          var value = kernel.Resolve(property.PropertyType);
          try {
            property.SetValue(target, value, null);
          }
          catch (Exception ex) {
            var message = string.Format("Error setting property {0} on type {1}, See inner exception for more information.", property.Name, type.FullName);
            throw new ComponentActivatorException(message, ex);
          }
        }
      }
    }
  }
}
