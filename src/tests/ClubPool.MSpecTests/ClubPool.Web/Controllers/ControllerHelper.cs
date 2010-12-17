using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using System.Web.Routing;
using System.Web;

using Rhino.Mocks;

using ClubPool.Web.Controllers;

namespace ClubPool.MSpecTests.ClubPool.Web.Controllers
{
  public static class ControllerHelper
  {
    public static HttpContextBase CreateMockHttpContext() {
      var mockIdentity = MockRepository.GenerateMock<IIdentity>();
      var mockPrincipal = MockRepository.GenerateMock<IPrincipal>();
      mockPrincipal.Stub(p => p.Identity).Return(mockIdentity);
      var mockHttpContext = MockRepository.GenerateMock<HttpContextBase>();
      mockHttpContext.Stub(c => c.User).Return(mockPrincipal);
      var requestContext = new RequestContext(mockHttpContext, new RouteData());
      var mockRequest = MockRepository.GenerateMock<HttpRequestBase>();
      mockHttpContext.Stub(c => c.Request).Return(mockRequest);
      mockHttpContext.Stub(c => c.CurrentHandler).Return(new MvcHandler(requestContext));
      return mockHttpContext;
    }

    public static ControllerContext CreateMockControllerContext(ControllerBase controller) {
      var mockHttpContext = CreateMockHttpContext();
      var controllerContext = new ControllerContext(mockHttpContext, new RouteData(), controller);
      controller.ControllerContext = controllerContext;
      return controllerContext;
    }

  }
}
