using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class IndexViewModel : ViewModelBase
  {
    public IEnumerable<SeasonDto> Seasons { get; set; }
    public int Page { get; set; }
    public int First { get; set; }
    public int Last { get; set; }
    public int Total { get; set; }
    public int LastPage { get; set; }
  }
}
