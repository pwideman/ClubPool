<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Meets.ViewModels.MeetViewPlayerViewModel>" %>

<div class="player-card">
  <div class="header"><%= Model.Name %></div>
  <div class="details">
    <div class="details-row">
      <div class="label">Team:</div>
      <div class="data"><%= Model.TeamName %></div>
    </div>
    <div class="details-row">
      <div class="label">Skill Level:</div>
      <div class="data"><%= Model.SkillLevel > 0 ? Model.SkillLevel.ToString() : "None" %></div>
    </div>
    <div class="details-row">
      <div class="label">Record:</div>
      <div class="data"><%= Model.Wins.ToString()%> - <%= Model.Losses.ToString()%> (<%= Model.WinPercentage.ToString(".00")%>)</div>
    </div>
  </div>
</div>
