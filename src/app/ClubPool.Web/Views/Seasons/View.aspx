﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Seasons.ViewModels.SeasonViewModel>" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Divisions" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Teams" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Seasons" %>
<%@ Import Namespace="ClubPool.Framework.Extensions" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span><%= Model.Name %></span>
  </div>

  <div class="action-button-row">
    <div class="action-button">
      <%= Html.ContentImage("add-medium.png", "Add Division") %>
      <%= Html.ActionLink<DivisionsController>(c => c.Create(Model.Id), "Add a new division to this season") %>
    </div>
  </div>

  <div id="divisiontabs">
    <ul>
      <% foreach (var division in Model.Divisions) { %>
      <li>
        <a href="#division-<%= division.Id %>"><%= division.Name%></a>
      </li>
      <% } %>
    </ul>
    <% foreach (var division in Model.Divisions) { %>
    <div id="division-<%= division.Id %>">
      <div class="action-button-row">
        <div class="action-button">
          <%= Html.ContentImage("edit-medium.png", "Edit Division") %>
          <%= Html.ActionLink<DivisionsController>(c => c.Edit(division.Id), "Edit this division") %>
        </div>
        <div class="action-button">
          <%= Html.ContentImage("add-medium.png", "Add Team") %>
          <%= Html.ActionLink<TeamsController>(c => c.Create(division.Id), "Add a new team to this division") %>
        </div>
        <% if (division.CanDelete) { %>
        <div class="action-button">
          <% using (var form = Html.BeginForm<DivisionsController>(c => c.Delete(division.Id), FormMethod.Post)) { %>
          <%= Html.AntiForgeryToken()%>
          <%= Html.ContentImage("delete-medium.png", "Delete Team") %>
          <a href="#" class="delete-form-link">Delete this division</a>
          <% } %>
        </div>     
        <% } %>
      </div>
      <div id="division-<%= division.Id %>-tabs" class="division-details-tabs">
        <ul>
          <li><a href="#division-<%= division.Id %>-teams">Teams</a></li>
          <li><a href="#division-<%= division.Id %>-schedule">Schedule</a></li>
        </ul>
        <div id="division-<%= division.Id %>-teams">
          <% if (division.Teams.Any()) { %>
            <table class="season-view-teams-table domain-list">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Players</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
              <% foreach (var team in division.Teams) { %>
                <tr>
                  <td><%= team.Id%></td>
                  <td><%= team.Name%></td>
                  <td>
                  <% if (team.Players.Any()) { %>
                    <div class="season-view-player-list">
                      <ul>
                      <% foreach (var player in team.Players) { %>
                        <li><%= player.Name%></li>
                      <% } %>
                      </ul>
                    </div>
                  <% } %>
                  </td>
                  <td class="action-column">
                    <a href="<%= Html.BuildUrlFromExpression<TeamsController>(c => c.Edit(team.Id)) %>">
                    <%= Html.ContentImage("edit-medium.png", "Edit")%>
                    </a>
                    <% if (team.CanDelete) {
                          using (var form = Html.BeginForm<TeamsController>(c => c.Delete(team.Id), FormMethod.Post, new { @class = "invisible" })) { %>
                      <input type="image" value="Delete" alt="Delete" src="<%= Url.ContentImageUrl("delete-medium.png")%>"/>
                      <%= Html.AntiForgeryToken()%>
                    <%   }
                        } %>
                  </td>
                </tr>
              <% } %>
              </tbody>
            </table>
          <% }
              else { %>
          This division has no teams
          <% } %>
        </div>
        <div id="division-<%= division.Id %>-schedule">
          <% if (division.HasSchedule) { %>
          <table class="schedule-table">
            <thead>
              <tr>
                <th>Week</th>
                <th>Date</th>
                <th colspan="<%= division.Schedule.NumberOfMeetsPerWeek %>">Matches</th>
              </tr>
            </thead>
            <% foreach (var week in division.Schedule.Weeks) { %>
            <tbody>
              <tr>
                <td><%= week.Week %></td>
                <td><%= week.Date.ToShortDateString() %></td>
              <% foreach (var meet in week.Meets) { %>
                <td><%= string.Join(" vs ", meet.TeamNames) %></td>
              <% } %>
              </tr>
            <% } %>
            </tbody>
          </table>
          <% } else { %>
          <p>The schedule for this division has not been created.</p>
          <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Divisions.DivisionsController>(c => c.CreateSchedule(division.Id), FormMethod.Post, new { @class = "normal" })) { %>
            <%= Html.AntiForgeryToken() %>
            <input class="submit-button" type="submit" value="Create Schedule" />
          <% } 
          } %>
        </div>
      </div>

    </div>
    <% } %>
  </div>

  <%
    if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
      Html.RenderPartial("ErrorMessage");
    }
    else if (TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage)) {
      Html.RenderPartial("NotificationMessage");
    }
  %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
<%= Model.Name %> - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      $("#divisiontabs").tabs({
        cookie: {}
      });
      $(".division-details-tabs").tabs({
        cookie: {}
      });
      $(".delete-form-link").click(function () {
        $(this).parents("form:first").submit();
      });
    });
  </script>
</asp:Content>
