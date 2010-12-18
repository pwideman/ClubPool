<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.AlertsViewModel>" %>
<div class="sidebar-gadget">
  <div class="alerts-gadget">
    <% if (!(Model.Errors.Any() || Model.Warnings.Any() || Model.Notifications.Any())) { %>
      <div class="no-alerts"><span>You have no alerts</span></div>
    <% } else { %>
    <% if (Model.Errors.Any()) { %>
    <div class="alerts-container errors">
      <ul>
        <% foreach (var alert in Model.Errors) { %>
        <li><a href="<%= alert.Url%>"><%= Html.Encode(alert.Message)%></a></li>
        <% } %>
      </ul>
    </div>
    <% } %>
    <% if (Model.Warnings.Any()) { %>
    <div class="alerts-container warnings">
      <ul>
        <% foreach (var alert in Model.Warnings) { %>
        <li><a href="<%= alert.Url%>"><%= Html.Encode(alert.Message)%></a></li>
        <% } %>
      </ul>
    </div>
    <% } %>
    <% if (Model.Notifications.Any()) { %>
    <div class="alerts-container notifications">
      <ul>
        <% foreach (var alert in Model.Notifications) { %>
        <li><a href="<%= alert.Url%>"><%= Html.Encode(alert.Message)%></a></li>
        <% } %>
      </ul>
    </div>
   <% }
   } %>
  </div>
</div>