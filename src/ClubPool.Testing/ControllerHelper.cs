using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using System.Web.Routing;
using System.Web;

using Moq;

using ClubPool.Web.Controllers;

namespace ClubPool.Testing
{
  public static class ControllerHelper
  {
    public static HttpContextBase CreateMockHttpContext() {
      var mockIdentity = new Mock<IIdentity>();
      var mockPrincipal = new Mock<IPrincipal>();
      mockPrincipal.Setup(p => p.Identity).Returns(mockIdentity.Object);
      var mockHttpContext = new Mock<HttpContextBase>();
      mockHttpContext.Setup(c => c.User).Returns(mockPrincipal.Object);
      var requestContext = new RequestContext(mockHttpContext.Object, new RouteData());
      var mockRequest = new Mock<HttpRequestBase>();
      mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
      mockHttpContext.Setup(c => c.CurrentHandler).Returns(new MvcHandler(requestContext));
      return mockHttpContext.Object;
    }

    public static ControllerContext CreateMockControllerContext(ControllerBase controller) {
      var mockHttpContext = CreateMockHttpContext();
      var controllerContext = new ControllerContext(mockHttpContext, new RouteData(), controller);
      controller.ControllerContext = controllerContext;
      return controllerContext;
    }

  }
}
