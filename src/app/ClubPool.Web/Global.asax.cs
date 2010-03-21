using System;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;
using MvcContrib.Castle;
using NHibernate.Cfg;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Event;
using SharpArch.Data.NHibernate;
using SharpArch.Web.NHibernate;
using SharpArch.Web.Castle;
using SharpArch.Web.Areas;
using SharpArch.Web.CommonValidator;
using SharpArch.Web.ModelBinder;

using ClubPool.Framework.Validation;
using ClubPool.Web.Controllers;
using ClubPool.Data.NHibernateMaps;
using ClubPool.Web.CastleWindsor;
using ClubPool.Web.Code;

namespace ClubPool.Web
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : HttpApplication
  {
    private WebSessionStorage webSessionStorage;

    protected void Application_Start() {
      
      log4net.Config.XmlConfigurator.Configure();

      //ModelBinders.Binders.DefaultBinder = new SharpModelBinder();

      // NHV shared engine provider
      // I added this to experiment with the fluent NHV config (loquacious),
      // which I'm no longer using. I'll leave it in place for now.
      var provider = new NHibernateSharedEngineProvider();
      NHibernate.Validator.Cfg.Environment.SharedEngineProvider = provider;
      var cfg = new FluentConfiguration();
      cfg.Register(typeof(ClubPool.Core.User).Assembly.ValidationDefinitions())
         .Register(typeof(HomeController).Assembly.ValidationDefinitions())
         .SetDefaultValidatorMode(NHibernate.Validator.Engine.ValidatorMode.OverrideAttributeWithExternal);
      NHibernate.Validator.Cfg.Environment.SharedEngineProvider.GetEngine().Configure(cfg);
      // xVal & the NHValidatorRulesProvider
      xVal.ActiveRuleProviders.Providers.Add(new NHValidatorRulesProvider());

      InitializeServiceLocator();

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
      ComponentRegistrar.AddComponentsTo(container);

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