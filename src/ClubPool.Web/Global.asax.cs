using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Data.Entity;
using System.Reflection;

using Autofac;
using Autofac.Integration.Mvc;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Infrastructure.EntityFramework;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure.Configuration;

namespace ClubPool.Web
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new ElmahHandleErrorLoggerFilter());
      filters.Add(new HandleErrorAttribute());
      filters.Add(new GlobalViewBagFilter(DependencyResolver.Current.GetService<ClubPoolConfiguration>()));
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      var routeRegistrars = DependencyResolver.Current.GetServices<IRouteRegistrar>();
      foreach (var routeRegistrar in routeRegistrars) {
        routeRegistrar.RegisterRoutes(routes);
      }

      // must do the default route last, otherwise it overrides everything else
      routes.MapRoute(
        "Default", // Route name
        "{controller}/{action}/{id}", // URL with parameters
        new { controller = "Home", action = "Index", id = UrlParameter.Optional }  // Parameter defaults
      );
   }

    protected void Application_Start() {
      Database.SetInitializer<ClubPoolContext>(null);

      InitializeServiceLocator(GetConfig());

      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }

    private ClubPoolConfiguration GetConfig() {
      var config = ClubPoolConfigurationSection.GetConfig();
      config.AppRootPath = AppDomain.CurrentDomain.BaseDirectory;
      config.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
      return config;
    }

    protected virtual void InitializeServiceLocator(ClubPoolConfiguration config) {
      var builder = new ContainerBuilder();
      builder.RegisterType<Repository>().As<IRepository>();
      builder.Register<Lazy<DbContext>>(c => new Lazy<DbContext>(() => new ClubPoolContext())).InstancePerHttpRequest();
      builder.Register<ScriptViewRegistrar>(c => new ScriptViewRegistrar()).InstancePerHttpRequest();
      builder.RegisterInstance(config);

      var assembly = typeof(MvcApplication).Assembly;
      builder.RegisterAssemblyTypes(assembly)
        .Where(t => t.Namespace.Contains("Services"))
        .AsImplementedInterfaces();
      builder.RegisterAssemblyTypes(assembly)
        .Where(t => t.IsAssignableTo<IRouteRegistrar>())
        .As<IRouteRegistrar>();

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

  }
}