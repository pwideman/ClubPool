<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Seasons.ViewModels.SeasonViewModel>" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Divisions" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Teams" %>
<%@ Import Namespace="ClubPool.Web.Controllers.Seasons" %>
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

  <div id="tabs">
    <ul>
      <% foreach (var division in Model.Divisions) { %>
      <li>
        <a href="#tab-<%= division.Id %>"><%= division.Name%></a>
      </li>
      <% } %>
    </ul>
    <% foreach (var division in Model.Divisions) { %>
    <div id="tab-<%= division.Id %>">
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
      <!--<div class="content-box season-view-teams-content">
        <div class="content-box-title">
          <span class="content-box-title-heading">Teams</span>
        </div>
        <div class="content-box-content">-->
      <div class="content-box" style="display: inline-block;">
        <div class="content-box-header">Teams</div>
        <div class="content-box-body">
        <% if (division.Teams.Any()) { %>
          <table class="season-view-teams-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Players</th>
                <th></th>
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
                </td>
                <td class="action-column">
                  <% if (team.CanDelete) {
                       using (var form = Html.BeginForm<TeamsController>(c => c.Delete(team.Id), FormMethod.Post)) { %>
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
      $("#tabs").tabs({
        cookie: {}
      });
      $(".delete-form-link").click(function () {
        $(this).parents("form:first").submit();
      });
    });
  </script>
</asp:Content>
