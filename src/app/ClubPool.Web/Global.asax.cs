using System;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Web.Security;

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Cfg;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Event;
using SharpArch.Core;
using SharpArch.Data.NHibernate;
using SharpArch.Web.NHibernate;
using SharpArch.Web.Castle;
using SharpArch.Web.Areas;
using SharpArch.Web.CommonValidator;
using SharpArch.Web.ModelBinder;
using log4net;

using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.Framework.NHibernate;
using ClubPool.Framework.Validation;
using ClubPool.Web.Controllers;
using ClubPool.Data.NHibernateMaps;
using ClubPool.Web.CastleWindsor;
using ClubPool.Web.Code;
using ClubPool.ApplicationServices.Authentication;

namespace ClubPool.Web
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : HttpApplication
  {
    private WebSessionStorage webSessionStorage;
    protected static readonly ILog logger = LogManager.GetLogger(typeof(MvcApplication));

    protected void Application_Start() {
      
      log4net.Config.XmlConfigurator.Configure();

      //ModelBinders.Binders.DefaultBinder = new SharpModelBinder();
      ModelBinders.Binders.DefaultBinder = new ModelBinder();

      // NHV shared engine provider
      // I added this to experiment with the fluent NHV config (loquacious),
      // which I'm no longer using. I'll leave it in place for now.
      var provider = new NHibernateSharedEngineProvider();
      NHibernate.Validator.Cfg.Environment.SharedEngineProvider = provider;
      var cfg = new FluentConfiguration();
      cfg.Register(typeof(ClubPool.Core.User).Assembly.ValidationDefinitions())
         .Register(typeof(ClubPool.Web.Controllers.Home.HomeController).Assembly.ValidationDefinitions())
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

      container.RegisterControllers(typeof(BaseController).Assembly);
      container.Install(FromAssembly.This());

    }

    public override void Init() {
      base.Init();

      // The WebSessionStorage must be created during the Init() to tie in HttpApplication events
      webSessionStorage = new WebSessionStorage(this);
    }

    /// <summary>
    /// Due to issues on IIS7, the NHibernate initialization cannot reside in Init() but
    /// must only be called once.  Consequently, we invoke a thread-safe singleton class to 
    /// ensure it's only initialized once.
    /// </summary>
    protected void Application_BeginRequest(object sender, EventArgs e) {
        NHibernateInitializer.Instance().InitializeNHibernateOnce(
          () => InitializeNHibernateSession());
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
        return;
      }

      FormsAuthenticationTicket authTicket = null;
      try {
        authTicket = FormsAuthentication.Decrypt(authCookie.Value);
      }
      catch (Exception ex) {
        logger.Error("Forms authentication ticket could not be decrypted", ex);
        return;
      }

      if (null == authTicket) {
        // Cookie failed to decrypt.
        return;
      }

      // Create an Identity object
      FormsIdentity id = new FormsIdentity(authTicket);

      var userRepository = SafeServiceLocator<IUserRepository>.GetService();
      var roles = userRepository.FindOne(UserQueries.UserByUsername(id.Name))
                                .Roles.Select(RoleQueries.SelectName).ToArray();
      // This principal will flow throughout the request.
      ClubPoolPrincipal principal = new ClubPoolPrincipal(id, roles);
      // Attach the new principal object to the current HttpContext object
      Context.User = principal;
    }

    /// <summary>
    /// If you need to communicate to multiple databases, you'd add a line to this method to
    /// initialize the other database as well.
    /// </summary>
    private void InitializeNHibernateSession() {
      NHibernateSession.Init(
          webSessionStorage,
          new string[] { Server.MapPath("~/bin/ClubPool.Data.dll") },
          new AutoPersistenceModelGenerator().Generate());
      NHibernateSession.ValidatorEngine = NHibernate.Validator.Cfg.Environment.SharedEngineProvider.GetEngine();
    }

  }
}