using System;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Web.Security;
using System.Data.Entity;

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Facilities.FactorySupport;
using Castle.Windsor.Installer;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Event;
using SharpArch.Core;
using SharpArch.Web.Castle;
using log4net;

using ClubPool.Framework.Validation;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Infrastructure.EntityFramework;
using ClubPool.Web.Controllers;
using ClubPool.Web.CastleWindsor;
using ClubPool.Web.Binders;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Models;

namespace ClubPool.Web
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : HttpApplication
  {
    protected static readonly ILog logger = LogManager.GetLogger(typeof(MvcApplication));

    protected void Application_Start() {
      
      log4net.Config.XmlConfigurator.Configure();

      ModelBinders.Binders.DefaultBinder = new ModelBinder();

      // NHV shared engine provider
      // I added this to experiment with the fluent NHV config (loquacious),
      // which I'm no longer using. I'll leave it in place for now.
      var provider = new NHibernateSharedEngineProvider();
      NHibernate.Validator.Cfg.Environment.SharedEngineProvider = provider;
      var cfg = new FluentConfiguration();
      cfg.Register(typeof(ClubPool.Web.Controllers.Home.HomeController).Assembly.ValidationDefinitions())
         .SetDefaultValidatorMode(NHibernate.Validator.Engine.ValidatorMode.OverrideAttributeWithExternal);
      NHibernate.Validator.Cfg.Environment.SharedEngineProvider.GetEngine().Configure(cfg);
      // xVal & the NHValidatorRulesProvider
      xVal.ActiveRuleProviders.Providers.Add(new NHValidatorRulesProvider());

      InitializeServiceLocator();

      AreaRegistration.RegisterAllAreas();
      RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);
    }

    /// <summary>
    /// Instantiate the container and add all Controllers that derive from 
    /// WindsorController to the container.  Also associate the Controller 
    /// with the WindsorContainer ControllerFactory.
    /// </summary>
    protected virtual void InitializeServiceLocator() {
      IWindsorContainer container = new WindsorContainer();
      // set up the ServiceLocator earlier so that we can use it in
      // ComponentRegistrar
      ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
      ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

      container.AddFacility<FactorySupportFacility>()
        .Register(
        Component.For<Lazy<DbContext>>()
        .UsingFactoryMethod(() => new Lazy<DbContext>(() => new ClubPoolContext()))
        .LifeStyle.PerWebRequest);
      //container.Register(Component.For<ClubPoolContext>().LifeStyle.PerWebRequest);
      container.Register(Component.For<IRepository>().ImplementedBy<Repository>().LifeStyle.Transient);
      container.RegisterControllers(typeof(BaseController).Assembly);
      container.Install(FromAssembly.This());

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

      var repository = SafeServiceLocator<IRepository>.GetService();
      var user = repository.All<User>().Single(u => u.Username.Equals(identity.Name));
      // This principal will flow throughout the request.
      ClubPoolPrincipal principal = new ClubPoolPrincipal(user, identity);
      // Attach the new principal object to the current HttpContext object
      Context.User = principal;
    }

    private void SetUnauthorizedPrincipal() {
      Context.User = ClubPoolPrincipal.CreateUnauthorizedPrincipal();
    }

    protected void Application_Error() {
      var exception = Server.GetLastError();
      // TODO: Version exceptions in EF?
      //if (exception is StaleObjectStateException) {
      //  this.Response.Redirect("~/home/staleobjectstateerror");
      //}
    }

  }
}