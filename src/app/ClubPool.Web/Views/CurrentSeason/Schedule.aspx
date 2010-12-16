<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.CurrentSeason.ViewModels.CurrentSeasonScheduleViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h4><%= Model.Name%> Schedule</h4>

<% if (!Model.HasDivisions) { %>
<p>This season has no divisions.</p>
<% } else { %>
<div id="divisiontabs">
  <ul>
  <% foreach (var division in Model.Divisions) { %>
    <li><a href="#division-<%= division.Id%>"><%= division.Name%></a></li>
  <% } %>
  </ul>
  <% foreach (var division in Model.Divisions) { %>
  <div id="division-<%= division.Id %>">
    <% if (division.HasSchedule) {
         Html.RenderPartial("ScheduleView", division.Schedule);
       } else { %>
    <p>This division has no schedule.</p>
    <% } %>
  </div>
  <% } %>
</div>
<% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<script type="text/javascript">
  $(function () {
    $("#divisiontabs").tabs();
  });
</script>
</asp:Content>
