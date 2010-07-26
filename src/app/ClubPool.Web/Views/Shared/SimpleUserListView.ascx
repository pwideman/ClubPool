<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ClubPool.Core.UserDto>>" %>
<div class="simple-user-list">
  <ul>
    <% foreach (var user in Model) { %>
    <li class="simple-user-list-item" id="<%= user.Id %>">
      <div class="simple-user-list-item-heading"><%= user.FullName %></div>
      <ul>
        <li><%= user.Username %></li>
        <li><%= user.Email %></li>
      </ul>
    </li>
    <% } %>
  </ul>
</div>
