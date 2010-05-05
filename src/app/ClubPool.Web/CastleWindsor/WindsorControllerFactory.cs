using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcContrib.Castle;
using Castle.Windsor;

namespace ClubPool.Web.CastleWindsor
{
  /// <summary>
  /// Controller Factory class for instantiating controllers using the Windsor IoC container.
  /// </summary>
  // This class is from MvcContrib (http://mvccontrib.codeplex.com). To change its behavior,
  // I have to copy the code and modify it instead of subclassing it, because they made the
  // container variable private. WHY?
  public class WindsorControllerFactory : DefaultControllerFactory
  {
    private IWindsorContainer _container;

    /// <summary>
    /// Creates a new instance of the <see cref="WindsorControllerFactory"/> class.
    /// </summary>
    /// <param name="container">The Windsor container instance to use when creating controllers.</param>
    public WindsorControllerFactory(IWindsorContainer container) {
      if (container == null) {
        throw new ArgumentNullException("container");
      }
      _container = container;
    }

    protected override IController GetControllerInstance(Type controllerType) {
      if (controllerType == null) {
        throw new HttpException(404, string.Format("The controller for path '{0}' could not be found or it does not implement IController.", RequestContext.HttpContext.Request.Path));
      }

      var icontroller = (IController)_container.Resolve(controllerType);

      // Changes from MvcContrib's version: get the IoC IActionInvoker and
      // set it on the controller
      var controller = icontroller as Controller;
      if (null != controller) {
        var invoker = _container.Resolve<IActionInvoker>();
        if (null != invoker) {
          controller.ActionInvoker = invoker;
        }
      }

      return icontroller;
    }

    public override void ReleaseController(IController controller) {
      var disposable = controller as IDisposable;

      if (disposable != null) {
        disposable.Dispose();
      }

      _container.Release(controller);
    }
  }
}
