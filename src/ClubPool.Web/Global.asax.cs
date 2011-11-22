using System;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Web.Security;
using System.Data.Entity;

using Autofac;
using Autofac.Integration.Mvc;
using log4net;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Infrastructure.Binders;
using ClubPool.Web.Infrastructure.EntityFramework;
using ClubPool.Web.Controllers;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Models;

namespace ClubPool.Web
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : HttpApplication
  {
    protected static readonly ILog logger = LogManager.GetLogger(typeof(MvcApplication));

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapRoute(
        "Default", // Route name
        "{controller}/{action}/{id}", // URL with parameters
        new { controller = "Home", action = "Index", id = UrlParameter.Optional }  // Parameter defaults
      );
    }

    protected void Application_Start() {
      log4net.Config.XmlConfigurator.Configure();

      ModelBinders.Binders.DefaultBinder = new ModelBinder();

      InitializeServiceLocator();

      AreaRegistration.RegisterAllAreas();

      // TODO: uncomment this once we figure out elmah with the new filters
      //RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }

    /// <summary>
    /// Instantiate the container and add all Controllers that derive from 
    /// WindsorController to the container.  Also associate the Controller 
    /// with the WindsorContainer ControllerFactory.
    /// </summary>
    protected virtual void InitializeServiceLocator() {
      var builder = new ContainerBuilder();
      // set up the ServiceLocator earlier so that we can use it in
      // ComponentRegistrar

      builder.RegisterType<Repository>().As<IRepository>();
      builder.Register<Lazy<DbContext>>(c => new Lazy<DbContext>(() => new ClubPoolContext())).InstancePerHttpRequest();

      var assembly = typeof(MvcApplication).Assembly;
      builder.RegisterAssemblyTypes(assembly)
        .Where(t => t.Namespace.Contains("Services"))
        .AsImplementedInterfaces();

      builder.RegisterControllers(assembly);

      var container = builder.Build();
      DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e) {
      // We need to set the HttpContext.User property to our own IPrincipal implementation
      // so that it uses our role service to satisfy IPrincipal.IsInRole(). The 
      // Authorize attribute uses this method to determine whether the authenticated user
      // is in the requested role

      // Extract the forms authentication cookie
      string cookieName = FormsAuthentication.FormsCookieName;
      HttpCookie authCookie = Context.Request.Cookies[cookieName];

      if (null == authCookie) {
        // There is no authentication cookie.
        SetUnauthorizedPrincipal();
        return;
      }

      FormsAuthenticationTicket authTicket = null;
      try {
        authTicket = FormsAuthentication.Decrypt(authCookie.Value);
      }
      catch (Exception ex) {
        logger.Error("Forms authentication ticket could not be decrypted", ex);
        SetUnauthorizedPrincipal();
        return;
      }

      if (null == authTicket) {
        // Cookie failed to decrypt.
        SetUnauthorizedPrincipal();
        return;
      }

      // Create an Identity object
      FormsIdentity identity = new FormsIdentity(authTicket);

      var repository = DependencyResolver.Current.GetService<IRepository>();
      var user = repository.All<User>().Single(u => u.Username.Equals(identity.Name));
      // This principal will flow throughout the request.
      ClubPoolPrincipal principal = new ClubPoolPrincipal(user, identity);
      // Attach the new principal object to the current HttpContext object
      Context.User = principal;
    }

    private void SetUnauthorizedPrincipal() {
      Context.User = ClubPoolPrincipal.CreateUnauthorizedPrincipal();
    }

    //protected void Application_Error() {
    //  var exception = Server.GetLastError();
    //  // TODO: Version exceptions in EF?
    //  if (exception is StaleObjectStateException) {
    //    this.Response.Redirect("~/home/staleobjectstateerror");
    //  }
    //}

  }
}