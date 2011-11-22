using System;
using System.Collections.Generic;

namespace ClubPool.Web.Controllers.Dashboard.ViewModels
{
  public class AlertsViewModel
  {
    public AlertsViewModel(IEnumerable<Alert> notifications, IEnumerable<Alert> warnings, IEnumerable<Alert> errors) {
      Warnings = warnings;
      Errors = errors;
      Notifications = notifications;
    }

    public IEnumerable<Alert> Warnings { get; protected set; }
    public IEnumerable<Alert> Errors { get; protected set; }
    public IEnumerable<Alert> Notifications { get; protected set; }
  }

  public class Alert
  {
    public Alert(string message, string url, AlertType type) {
      Message = message;
      Url = url;
      Type = type;
    }

    public AlertType Type { get; protected set; }
    public string Message { get; protected set; }
    public string Url { get; protected set; }
  }

  public enum AlertType
  {
    Notification,
    Warning,
    Error
  }
}
