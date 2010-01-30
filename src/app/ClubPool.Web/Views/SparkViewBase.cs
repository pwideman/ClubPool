using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Spark.Web.Mvc;
using MvcContrib.FluentHtml.Behaviors;
using MvcContrib.FluentHtml;

namespace ClubPool.Web.Views
{
  // this class mostly copied from the WhoCanHelpMe (http://whocanhelpme.codeplex.com/) SparkModelViewBase
  // It is needed so that we have a view that implements MvcContrib.FluentHtml.IViewModelContainer, so 
  // we can use the strongly typed HtmlHelper extensions and such in MvcContrib.FluentHtml
  public abstract class SparkViewBase<T> : SparkView<T>, IViewModelContainer<T> where T:class
  {
    protected readonly List<IBehaviorMarker> behaviors = new List<IBehaviorMarker>();

    protected SparkViewBase() {
      behaviors.Add(new ValidationBehavior(() => ViewData.ModelState));
    }

    #region IViewModelContainer<T> Members

    public string HtmlNamePrefix { get; set; }

    public T ViewModel {
      get { return Model; }
    }

    #endregion

    #region IBehaviorsContainer Members

    public IEnumerable<IBehaviorMarker> Behaviors {
      get { return behaviors; }
    }

    #endregion
  }
}
