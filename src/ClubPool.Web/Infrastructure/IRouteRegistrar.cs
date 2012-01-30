using System;
using System.Web.Routing;

namespace ClubPool.Web.Infrastructure
{
  public interface IRouteRegistrar
  {
    void RegisterRoutes(RouteCollection routes);
  }
}
