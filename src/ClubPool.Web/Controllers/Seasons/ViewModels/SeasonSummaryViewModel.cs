using System;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class SeasonSummaryViewModel
  {
    public SeasonSummaryViewModel(Season season) {
      Id = season.Id;
      Name = season.Name;
      IsActive = season.IsActive;
      CanDelete = season.CanDelete();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool CanDelete { get; protected set; }
  }
}
