﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Divisions.ViewModels
{
  public class CreateDivisionViewModel : ValidatableViewModel
  {
    public string SeasonName { get; set; }

    [Min(1)]
    public int SeasonId { get; set; }

    [DisplayName("Name:")]
    [NotNullNotEmpty(Message="Required")]
    public string Name { get; set; }

    [DisplayName("Starting date:")]
    [NotNullNotEmpty(Message="Required")]
    public string StartingDate { get; set; }
  }

  public class EditDivisionViewModel : CreateDivisionViewModel
  {
    [Min(1)]
    public int Id { get; set; }
  }
}