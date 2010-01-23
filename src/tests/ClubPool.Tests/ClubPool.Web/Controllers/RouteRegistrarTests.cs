﻿using NUnit.Framework;
using ClubPool.Web.Controllers;
using MvcContrib.TestHelper;
using System.Web.Routing;

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
