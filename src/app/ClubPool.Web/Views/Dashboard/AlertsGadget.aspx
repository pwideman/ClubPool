<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.AlertsGadgetViewModel>" %>
<% if (Model.Alerts.Count() > 0) { %>
<div class="ui-state-error ui-corner-all">
  <p><span class="ui-icon ui-icon-alert form-error-icon"></span>Alerts</p>
  <div class="alerts-gadget">
    <ul>
      <% foreach (var alert in Model.Alerts) { %>
      <li><a href="<%= alert.Url%>"><%= alert.Message %></a></li>
      <% } %>
    </ul>
  </div>
</div>
<% } %>
