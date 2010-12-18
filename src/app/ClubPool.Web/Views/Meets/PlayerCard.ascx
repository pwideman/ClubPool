<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Meets.ViewModels.MeetViewPlayerViewModel>" %>

<div class="player-card">
  <div class="header"><%= Html.Encode(Model.Name) %></div>
  <div class="details">
    <div class="details-row">
      <div class="label">Team:</div>
      <div class="data"><%= Html.Encode(Model.TeamName) %></div>
    </div>
    <div class="details-row">
      <div class="label">Skill Level:</div>
      <div class="data"><%= Model.SkillLevel > 0 ? Model.SkillLevel.ToString() : "None" %></div>
    </div>
    <div class="details-row">
      <div class="label">Record:</div>
      <div class="data"><%= Model.Wins%> - <%= Model.Losses%> (<%= Model.WinPercentage.ToString(".00")%>)</div>
    </div>
  </div>
</div>
