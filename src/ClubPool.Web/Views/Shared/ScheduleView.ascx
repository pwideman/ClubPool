﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Shared.ViewModels.ScheduleViewModel>" %>
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
      <td <%= meet.Highlight ? @"class=""highlight""" : "" %>>
        <%= Html.ActionLink(meet.Team1Name + " vs " + meet.Team2Name, "View", "Meets", new { id = meet.Id }, null)%>
        <div class="schedule-links"><a href="<%= Url.Action("Scoresheet", "Meets", new { id = meet.Id })%>"><%= Html.ContentImage("printer-medium.png", "Print a scoresheet") %></a></div>
      </td>
    <% } %>
    </tr>
  <% } %>
  </tbody>
</table>
