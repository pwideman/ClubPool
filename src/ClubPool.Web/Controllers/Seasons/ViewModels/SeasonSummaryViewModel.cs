using System;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class SeasonSummaryViewModel
  {
    public SeasonSummaryViewModel() {
    }

    public Season Season {
      set {
        Id = value.Id;
        Name = value.Name;
        IsActive = value.IsActive;
        CanDelete = value.CanDelete();
      }
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool CanDelete { get; protected set; }
  }
}
