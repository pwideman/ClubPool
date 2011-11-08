using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Framework.Validation;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class SignUpViewModel : CreateViewModel
  {
    public string SiteName { get; set; }
  }
}
