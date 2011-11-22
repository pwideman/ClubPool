using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace ClubPool.Web.Controllers.Extensions
{
  public static class ModelStateDictionaryExtensions
  {
    public static void AddModelErrorFor<TModel>(this ModelStateDictionary modelState,
      Expression<Func<TModel, object>> expression,
      string message) where TModel : class {

        modelState.AddModelError(ExpressionHelper.GetExpressionText(expression), message);
    }
  }
}
