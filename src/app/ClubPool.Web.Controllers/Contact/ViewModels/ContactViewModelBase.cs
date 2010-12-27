using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Web.Controllers.Contact.ViewModels
{
  public abstract class ContactViewModelBase : ValidatableViewModel
  {
    [NotNullNotEmpty]
    [Email]
    [DisplayName("Reply to Address:")]
    public string ReplyToAddress { get; set; }

    [NotNullNotEmpty]
    [DisplayName("Subject:")]
    public string Subject { get; set; }

    [NotNullNotEmpty]
    [DisplayName("Body:")]
    public string Body { get; set; }
  }
}
