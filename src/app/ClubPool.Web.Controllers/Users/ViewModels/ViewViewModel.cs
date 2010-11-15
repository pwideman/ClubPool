﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

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

    public ViewViewModel() {
      Roles = new string[0];
    }

    public ViewViewModel(User user)
      : this() {

      Username = user.Username;
      Name = user.FullName;
      Email = user.Email;
      IsApproved = user.IsApproved;
      IsLocked = user.IsLocked;
      Roles = user.Roles.Select(r => r.Name).ToArray();
      SkillLevel = user.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).Select(sl => sl.Value).FirstOrDefault();
    }

  }
}