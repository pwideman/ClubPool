<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ClubPool.Core.UserDto>>" %>
<div class="simple-user-list">
  <% foreach (var user in Model) { %>
  <div class="simple-user-list-item">
    <div class="simple-user-list-item-heading"><%= user.FullName %></div>
    <ul>
      <li><%= user.Username %></li>
      <li><%= user.Email %></li>
    </ul>
  </div>
  <% } %>
</div>
