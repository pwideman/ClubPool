using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Testing.Core
{
  public class TestMeet : Meet
  {
    public TestMeet(Team team1, Team team2, int week, Division division) : base(team1, team2, week) {
      Division = division;
    }
  }
}
