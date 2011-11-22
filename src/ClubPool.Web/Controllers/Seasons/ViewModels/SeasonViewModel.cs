using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class SeasonViewModel
  {
    public SeasonViewModel() {
      Divisions = new List<DivisionViewModel>();
    }
    
    public SeasonViewModel(Season season) {
      Id = season.Id;
      Name = season.Name;
      Divisions = season.Divisions.Select(d => new DivisionViewModel(d)).ToList();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<DivisionViewModel> Divisions { get; set; }

  }
}
