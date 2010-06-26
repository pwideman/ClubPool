using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class IndexViewModel : PagedListViewModelBase
  {
    public IEnumerable<SeasonDto> Seasons { get; set; }
  }
}
