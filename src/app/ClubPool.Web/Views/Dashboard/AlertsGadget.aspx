<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.AlertsGadgetViewModel>" %>
<% if (Model.Alerts.Any()) { %>
<div class="ui-state-error ui-corner-all sidebar-gadget">
  <p><span class="ui-icon ui-icon-alert message-icon"></span>Alerts</p>
  <div class="alerts-gadget">
    <ul>
      <% foreach (var alert in Model.Alerts) { %>
      <li><a href="<%= alert.Url%>"><%= alert.Message %></a></li>
      <% } %>
    </ul>
  </div>
</div>
<% } %>
