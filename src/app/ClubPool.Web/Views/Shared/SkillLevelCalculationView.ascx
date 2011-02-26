<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Shared.ViewModels.SkillLevelCalculationViewModel>" %>

<table id="skill_level_calc" class="results">
  <thead>
    <tr>
      <th>Date</th>
      <th>Team</th>
      <th>Player</th>
      <th>I</th>
      <th>D</th>
      <th>Net</th>
      <th>W</th>
    </tr>
  </thead>
  <tbody class="skill_level_matches">
  <% foreach (var result in Model.SkillLevelMatchResults) { %>
    <tr <%= result.Included ? @"class=""included""" : "" %>>
      <td><%= string.Format("{0} {1}", result.Date.ToShortDateString(), result.Date.ToShortTimeString()) %></td>
      <td class="team"><%= Html.Encode(result.Team) %></td>
      <td class="player"><%= Html.Encode(result.Player) %></td>
      <td><%= result.Innings %></td>
      <td><%= result.DefensiveShots %></td>
      <td><%= result.NetInnings %></td>
      <td><%= result.Wins %></td>
    </tr>
  <% } %>
  </tbody>
  <tbody class="skill_level_totals">
    <tr>
      <td colspan="2">Totals</td>
      <td><%= string.Format("I/G: {0:.000}", Model.TotalIG) %></td>
      <td><%= Model.TotalInnings %></td>
      <td><%= Model.TotalDefensiveShots %></td>
      <td><%= Model.TotalNetInnings %></td>
      <td><%= Model.TotalWins %></td>
    </tr>
    <tr>
      <td colspan="2">Culled Totals</td>
      <td><%= string.Format("I/G: {0:.000}", Model.CulledIG) %></td>
      <td><%= Model.TotalCulledInnings %></td>
      <td><%= Model.TotalCulledDefensiveShots%></td>
      <td><%= Model.TotalCulledNetInnings%></td>
      <td><%= Model.TotalCulledWins%></td>
    </tr>
    <tr>
      <td colspan="7">Skill Level: <%= Model.EightBallSkillLevel%></td>
    </tr>
  </tbody>
</table>
