using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcContrib.FluentHtml.Behaviors;
using MvcContrib.FluentHtml;

namespace ClubPool.Web.Views
{
  public class AspxViewPageBase<T> : ViewPage<T>, IViewModelContainer<T> where T : class
  {
    protected readonly List<IBehaviorMarker> behaviors = new List<IBehaviorMarker>();

    protected AspxViewPageBase() {
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

  public class AspxViewUserControlBase<T> : ViewUserControl<T>, IViewModelContainer<T> where T : class
  {
    protected readonly List<IBehaviorMarker> behaviors = new List<IBehaviorMarker>();

    protected AspxViewUserControlBase() {
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
