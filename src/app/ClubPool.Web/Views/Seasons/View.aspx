<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Seasons.ViewModels.SeasonViewModel>" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Divisions" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Teams" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Seasons" %>
<%@ Import Namespace="ClubPool.Framework.Extensions" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span><%= Html.Encode(Model.Name) %></span>
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
        <a href="#division-<%= division.Id %>"><%= Html.Encode(division.Name)%></a>
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
          <a href="#" class="submit-form-link">Delete this division</a>
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
              <tbody class="content mouseover-highlight-row">
              <% foreach (var team in division.Teams) { %>
                <tr>
                  <td><%= team.Id%></td>
                  <td><%= Html.ActionLink<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Details(team.Id), team.Name) %></td>
                  <td>
                  <% if (team.Players.Any()) { %>
                    <div class="season-view-player-list">
                      <ul>
                      <% foreach (var player in team.Players) { %>
                        <li><%= Html.Encode(player.Name)%></li>
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
                    <%    }
                       } %>
                  </td>
                </tr>
              <% } %>
              </tbody>
            </table>
          <% } else { %>
            <p>This division has no teams.</p>
          <% } %>
        </div>
        <div id="division-<%= division.Id %>-schedule">
          <% if (division.HasEnoughTeamsForSchedule) { %>
            <% if (division.HasSchedule) { %>
              <div class="action-button-row">
                <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Divisions.DivisionsController>(c => c.RecreateSchedule(division.Id), FormMethod.Post, new { @class = "inline" })) { %>
                  <%= Html.AntiForgeryToken()%>
                  <div class="action-button">
                    <%= Html.ContentImage("refresh-medium.png", "Recreate Schedule")%>
                    <a href="#" class="submit-form-link">Recreate the schedule</a>
                  </div>
                <% } %>
                <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Divisions.DivisionsController>(c => c.ClearSchedule(division.Id), FormMethod.Post, new { @class = "inline" })) { %>
                <%= Html.AntiForgeryToken()%>
                <div class="action-button">
                  <%= Html.ContentImage("delete-medium.png", "Clear Schedule")%>
                  <a href="#" class="submit-form-link">Clear the schedule</a>
                </div>
                <% } %>
              </div>
              <% Html.RenderPartial("ScheduleView", division.Schedule); %>
              <% }
               else { %>
              <p>This division does not have a schedule.</p>
              <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Divisions.DivisionsController>(c => c.CreateSchedule(division.Id), FormMethod.Post, new { @class = "normal" })) { %>
                <%= Html.AntiForgeryToken()%>
                <input class="submit-button" type="submit" value="Create Schedule" />
              <% }
               } %>
          <% } else { %>
            <p>This division doesn't have enough teams to have a schedule.</p>
          <% } %>
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
<%= Html.Encode(Model.Name) %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
  <script type="text/javascript">
    $(function () {
      $("tbody.content tr:odd").addClass("alt");
      $("#divisiontabs").tabs({
        cookie: {}
      });
      $(".division-details-tabs").tabs({
        cookie: {}
      });
      $(".submit-form-link").click(function () {
        $(this).parents("form:first").submit();
      });
    });
  </script>
</asp:Content>
