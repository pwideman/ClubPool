<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Shared.ViewModels.ScheduleViewModel>" %>
<table class="schedule-table">
  <thead>
    <tr>
      <th>Week</th>
      <th>Date</th>
      <th colspan="<%= Model.NumberOfMeetsPerWeek %>">Matches</th>
    </tr>
  </thead>
  <% foreach (var week in Model.Weeks) { %>
  <tbody>
    <tr>
      <td><%= week.Week%></td>
      <td><%= week.Date.ToShortDateString()%></td>
    <% foreach (var meet in week.Meets) { %>
      <td><%= Html.ActionLink<ClubPool.Web.Controllers.Meets.MeetsController>(c => c.View(meet.Id), meet.Team1Name + " vs " + meet.Team2Name)%></td>
    <% } %>
    </tr>
  <% } %>
  </tbody>
</table>
