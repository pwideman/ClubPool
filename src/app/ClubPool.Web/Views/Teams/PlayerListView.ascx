<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ClubPool.Web.Controllers.Teams.ViewModels.PlayerViewModel>>" %>
<div class="simple-user-list">
  <ul>
    <% foreach (var player in Model) { %>
    <li class="simple-user-list-item" id="<%= player.Id %>">
      <div class="simple-user-list-item-heading"><%= player.Name%></div>
      <ul>
        <li><%= player.Username%></li>
        <li><%= player.Email%></li>
      </ul>
    </li>
    <% } %>
  </ul>
</div>
