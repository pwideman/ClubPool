<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Navigation.ViewModels.MenuViewModel>" %>

<ul class="sf-menu">
	<li><%= Html.ActionLink<ClubPool.Web.Controllers.Home.HomeController>(c => c.Index(), "Home") %></li>
  <li><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(x => x.Login(string.Empty), "Login")%></li>
  <li><a href="#">League</a>
    <ul>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Home.HomeController>(c => c.Rules(), "Rules") %></li>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Home.HomeController>(c => c.Regulations(), "Regulations") %></li>
    </ul>
  </li>
  <% if (Model.UserIsLoggedIn && Model.HasActiveSeason) { %>
  <li><a href="#"><%= Html.Encode(Model.ActiveSeasonName) %></a>
    <ul>
      <% if (Model.HasCurrentTeam) { %>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Details(Model.CurrentTeamId), "My Team")%></li>
      <% } %>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.CurrentSeason.CurrentSeasonController>(c => c.Schedule(), "Schedule")%></li>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.CurrentSeason.CurrentSeasonController>(c => c.Standings(), "Standings") %></li>
    </ul>
  </li>
  <% } %>
  <% if (Model.UserIsLoggedIn) { %>
  <li><a href="#">My</a>
    <ul>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Dashboard.DashboardController>(c => c.Index(), "Home") %></li>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Matches.MatchesController>(c => c.UserHistory(Model.UserId, null), "Match History")%></li>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.Edit(Model.UserId), "Member Info") %></li>
    </ul>
  </li>
  <% } 
    if (Model.DisplayAdminMenu) { %>
  <li><a href="#">Admin</a>
    <ul>
      <% if (Model.HasActiveSeason) { %>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.View(Model.ActiveSeasonId), "Active Season")%></li>
      <% } %>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.Index(null), "Users") %></li>
      <li><%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(null), "Seasons") %></li>
      <li><a href="<%= Url.Content("~/elmah.axd")%>">Error Log</a></li>
    </ul>
  </li>
  <% } %>
  <li><%= Html.ActionLink<ClubPool.Web.Controllers.Home.HomeController>(c => c.About(), "About") %></li>
</ul>
