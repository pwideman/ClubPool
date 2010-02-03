using System.Web.Routing;

using NUnit.Framework;
using MvcContrib.TestHelper;

using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Home;

namespace Tests.ClubPool.Controllers
{
  [TestFixture]
  public class RouteRegistrarTests
  {
    [SetUp]
    public void SetUp() {
      RouteTable.Routes.Clear();
      RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);
    }

    [Test]
    public void CanVerifyRouteMaps() {
      "~/".Route().ShouldMapTo<HomeController>(x => x.Index());
    }
  }
}
