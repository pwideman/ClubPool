using System;

namespace ClubPool.Web.Controllers.Navigation
{
  public class MenuViewModel
  {
    public bool DisplayAdminMenu { get; set; }
    public bool UserIsLoggedIn { get; set; }
    public int UserId { get; set; }
    public bool HasActiveSeason { get; set; }
    public int ActiveSeasonId { get; set; }
    public string ActiveSeasonName { get; set; }
    public bool HasCurrentTeam { get; set; }
    public int CurrentTeamId { get; set; }
  }
}
