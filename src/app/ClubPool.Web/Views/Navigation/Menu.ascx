<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Navigation.ViewModels.MenuViewModel>" %>

<ul class="sf-menu">
	<li>
		<a href="/home">Home</a>
  </li>
  <li><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(x => x.Login(string.Empty), "Login")%></li>
  <li><a href="#">League</a>
    <ul>
      <li><a href="/league/regulations">Regulations</a></li>
      <li><a href="/league/rules">Rules</a></li>
    </ul>
  </li>
  <% if (Model.HasActiveSeason) { %>
  <li><a href="#">Current Season</a>
  </li>
  <% } %>
  <% if (Model.UserIsLoggedIn) { %>
  <li><a href="#">My</a>
    <ul>
      <li><a href="/dashboard">Home</a></li>
      <li><a href="/dashboard/team">Team</a></li>
      <li><a href="/dashboard/stats">Stats</a></li>
      <li><a href="/dashboard/history">Match History</a></li>
      <li><a href="/user/info">Member Info</a></li>
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
    </ul>
  </li>
  <% } %>
</ul>
