using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Web.Controllers.Shared.ViewModels;
using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class ViewViewModel
  {
    public string[] Roles { get; protected set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; }
    public bool IsLocked { get; set; }
    public int SkillLevel { get; set; }
    public bool HasSkillLevel { get; set; }
    public bool ShowAdminProperties { get; set; }
    public int Id { get; set; }
    public SkillLevelCalculationViewModel SkillLevelCalculation { get; set; }

    public ViewViewModel() {
      Roles = new string[0];
    }

    public ViewViewModel(User user, IRepository repository)
      : this() {

      Id = user.Id;
      Username = user.Username;
      Name = user.FullName;
      Email = user.Email;
      IsApproved = user.IsApproved;
      IsLocked = user.IsLocked;
      Roles = user.Roles.Select(r => r.Name).ToArray();
      if (user.SkillLevels.Any()) {
        SkillLevel = user.SkillLevels.Single(sl => sl.GameType == GameType.EightBall).Value;
        SkillLevelCalculation = new SkillLevelCalculationViewModel(user, repository);
        HasSkillLevel = true;
      }
      else {
        HasSkillLevel = false;
      }
    }

  }
}
