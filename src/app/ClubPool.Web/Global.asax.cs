using System;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;

using Spark;
using Spark.Web.Mvc;
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
    protected void Application_Start() {

      showCustomErrorPages = Convert.ToBoolean(ConfigurationManager.AppSettings["showCustomErrorPages"]);
      
      log4net.Config.XmlConfigurator.Configure();

      var spark = Convert.ToBoolean(ConfigurationManager.AppSettings["useSparkViewEngine"]);
      if (spark) {
        InitSparkViewEngine();
      }

      //ModelBinders.Binders.DefaultBinder = new SharpModelBinder();

      // NHV shared engine provider
      // I added this to experiment with the fluent NHV config (loquacious),
      // which I'm no longer using. I'll leave it in place for now.
      var provider = new NHibernateSharedEngineProvider();
      NHibernate.Validator.Cfg.Environment.SharedEngineProvider = provider;
      var cfg = new FluentConfiguration();
      cfg.Register(typeof(ClubPool.Core.Player).Assembly.ValidationDefinitions())
         .Register(typeof(ClubPool.SharpArchProviders.SharpArchMembershipProvider).Assembly.ValidationDefinitions())
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
          new string[] { Server.MapPath("~/bin/ClubPool.Data.dll"), Server.MapPath("~/bin/ClubPool.SharpArchProviders.dll") },
          new AutoPersistenceModelGenerator().Generate());
      NHibernateSession.ValidatorEngine = NHibernate.Validator.Cfg.Environment.SharedEngineProvider.GetEngine();
    }

    protected void Application_Error(object sender, EventArgs e) {
      // Useful for debugging
      var exception = Server.GetLastError();

      var context = HttpContext.Current;

      //  Show custom error page if necessary - from Who Can Help Me?
      if (showCustomErrorPages) {
        if (exception is HttpRequestValidationException) {
          this.DisplayErrorPage("InvalidInput");
          return;
        }

        this.DisplayErrorPage("Error");
      }
    }

    /// <summary>
    /// Returns a response by executing the Error controller with the specified action. 
    /// Shamelessly copied from the Who Can Help Me? showcase app
    /// </summary>
    /// <param name="action">
    /// The action.
    /// </param>
    private void DisplayErrorPage(string action) {
      var routeData = new RouteData();
      routeData.Values.Add("controller", "Error");
      routeData.Values.Add("action", action);

      this.Server.ClearError();
      this.Response.Clear();

      var httpContext = new HttpContextWrapper(this.Context);
      var requestContext = new RequestContext(httpContext, routeData);

      IController errorController = ControllerBuilder.Current.GetControllerFactory().CreateController(new RequestContext(httpContext, routeData), "Error");

      // Clear the query string, in particular to avoid HttpRequestValidationException being re-raised
      // when the error view is rendered by the Error Controller.
      httpContext.RewritePath(httpContext.Request.FilePath, httpContext.Request.PathInfo, string.Empty);

      errorController.Execute(requestContext);
    }

    // spark stuff
    public void InitSparkViewEngine() {
      ViewEngines.Engines.Clear();
      SparkEngineStarter.RegisterViewEngine(SparkInitializer.GetSettings());
      LoadPrecompiledViews(ViewEngines.Engines);
    }

    public void LoadPrecompiledViews(ViewEngineCollection engines) {
      SparkViewFactory factory = engines.OfType<SparkViewFactory>().First();
      factory.Engine.LoadBatchCompilation(Assembly.Load("ClubPool.Web.Views"));
    }
    // end spark stuff

    private WebSessionStorage webSessionStorage;
    private static bool showCustomErrorPages;
  }
}