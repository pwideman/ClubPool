﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Meets.ViewModels
{
  public class PlayerViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int SkillLevel { get; set; }
    public string TeamName { get; set; }
    public int GamesToWin { get; set; }

    public PlayerViewModel() {
    }

    public PlayerViewModel(User player, Team team) {
      Id = player.Id;
      Name = player.FullName;
      TeamName = team.Name;

      var gameType = team.Division.Season.GameType;
      var slQuery = player.SkillLevels.Where(sl => sl.GameType == gameType);
      if (slQuery.Any()) {
        SkillLevel = slQuery.First().Value;
      }
      else {
        SkillLevel = 0;
      }
    }
  }
}