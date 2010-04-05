using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Dashboard.ViewModels
{
  public class AlertsGadgetViewModel
  {
    public AlertsGadgetViewModel(IEnumerable<Alert> alerts) {
      Alerts = alerts;
    }

    public IEnumerable<Alert> Alerts { get; protected set; }
  }

  public class Alert
  {
    public Alert(string message, string url) {
      Message = message;
      Url = url;
    }

    public string Message { get; protected set; }
    public string Url { get; protected set; }
  }
}
