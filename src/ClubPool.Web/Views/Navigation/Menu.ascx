<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Navigation.ViewModels.MenuViewModel>" %>

<ul class="sf-menu">
  <li><%= Html.ActionLink("Home", "Index", "Home") %></li>
  <li><%= Html.ActionLink("Login", "Login", "Users")%></li>
  <% if (Model.UserIsLoggedIn) { %>
  <li><a href="#">League</a>
    <ul>
      <li><%= Html.ActionLink("Rules", "Rules", "Home")%></li>
      <li><%= Html.ActionLink("Regulations", "Regulations", "Home")%></li>
    </ul>
  </li>
  <% } %>
  <% if (Model.UserIsLoggedIn && Model.HasActiveSeason) { %>
  <li><a href="#"><%= Html.Encode(Model.ActiveSeasonName) %></a>
    <ul>
      <% if (Model.HasCurrentTeam) { %>
      <li><%= Html.ActionLink("My Team", "Details", "Teams", new { id = Model.CurrentTeamId }, null)%></li>
      <% } %>
      <li><%= Html.ActionLink("Schedule", "Schedule", "CurrentSeason")%></li>
      <li><%= Html.ActionLink("Standings", "Standings", "CurrentSeason") %></li>
    </ul>
  </li>
  <% } %>
  <% if (Model.UserIsLoggedIn) { %>
  <li><a href="#">My</a>
    <ul>
      <li><%= Html.ActionLink("Home", "Index", "Dashboard") %></li>
      <li><%= Html.ActionLink("Match History", "UserHistory", "Matches", new { id = Model.UserId }, null)%></li>
      <li><%= Html.ActionLink("Member Info", "Edit", "Users", new { id = Model.UserId }, null)%></li>
    </ul>
  </li>
  <% } 
    if (Model.DisplayAdminMenu) { %>
  <li><a href="#">Admin</a>
    <ul>
      <% if (Model.HasActiveSeason) { %>
      <li><%= Html.ActionLink("Active Season", "View", "Seasons", new { id = Model.ActiveSeasonId }, null)%></li>
      <% } %>
      <li><%= Html.ActionLink("Users", "Index", "Users") %></li>
      <li><%= Html.ActionLink("Seasons", "Index", "Seasons") %></li>
      <li><a href="<%= Url.Content("~/elmah.axd")%>">Error Log</a></li>
    </ul>
  </li>
  <% } %>
  <li><%= Html.ActionLink("About", "About", "Home") %></li>
</ul>
