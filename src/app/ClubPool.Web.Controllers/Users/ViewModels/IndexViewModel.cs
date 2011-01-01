﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MvcContrib.Pagination;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class IndexViewModel : PagedListViewModelBase<UserSummaryViewModel>
  {
    public string SearchQuery { get; set; }

    public IndexViewModel(IQueryable<UserSummaryViewModel> users, int page, int pageSize)
      : base(users, page, pageSize) {
    }
  }

}
