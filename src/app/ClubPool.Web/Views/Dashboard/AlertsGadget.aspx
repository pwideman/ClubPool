<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.AlertsGadgetViewModel>" %>
<% if (Model.Alerts.Count() > 0) { %>
<div class="sidebarGadgetContainer sidebarRoundCorners">
  <div class="sidebarGadgetTitle">Alerts</div>
  <div class="sidebarGadgetContent sidebarRoundCorners">
    <ul>
      <% foreach (var alert in Model.Alerts) { %>
      <li><a href="<%= alert.Url%>"><%= alert.Message %></a></li>
      <% } %>
    </ul>
  </div>
</div>
<% } %>
