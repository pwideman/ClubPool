<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Core.SeasonDto>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span><%= Model.Name %></span>
  </div>
  <div class="edit-season-divisions-list">
    <div class="form-title">Divisions</div>
    <div style="height: 80%;">
    <% if (0 == Model.Divisions.Length) { %>
    <div class="form-title">This season has no divisions</div>
    <% }
       else {
         foreach (var division in Model.Divisions) { %>
          <div>
            <%= division.Name%>
          </div>
      <% } %>
    <% } %>
    </div>
    <div class="action-button-row">
      <div class="action-button">
        <%= Html.ContentImage("add-medium.png", "Add Division") %>
        <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Create(), "Add a new division to this season") %>
      </div>
    </div>
  </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
