<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Navigation.ViewModels.MenuViewModel>" %>

<ul class="sf-menu">
	<li>
		<a href="/home">Home</a>
  </li>
  <li><%= Html.ActionLink<ClubPool.Web.Controllers.UserController>(x => x.Login(string.Empty), "Login") %></li>
  <li><a href="#">League</a>
    <ul>
      <li><a href="/league/regulations">Regulations</a></li>
      <li><a href="/league/rules">Rules</a></li>
    </ul>
  </li>
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
      <li><a href="/players">Players</a></li>
      <li><a href="/seasons">Seasons</a></li>
      <li><a href="/divisions">Divisions</a></li>
      <li><a href="/teams">Teams</a></li>
      <li><a href="/schedules">Schedules</a></li>
      <li><a href="/matches">Matches</a></li>
    </ul>
  </li>
  <% } %>
</ul>
