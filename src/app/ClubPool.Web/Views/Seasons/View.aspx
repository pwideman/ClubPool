<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Core.SeasonDto>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span><%= Model.Name %></span>
  </div>
  <div class="content-box season-view-divisions-list">
    <div class="content-box-title">
      <span class="content-box-title-heading">Divisions</span>
    </div>
    <div class="content-box-content">
    <% if (0 == Model.Divisions.Length) { %>
      <div>This season has no divisions</div>
    <% }
       else {
         foreach (var division in Model.Divisions) { %>
          <div class="content-box">
            <div class="content-box-title">
              <span class="content-box-title-heading"><%= division.Name%></span>
              <div class="content-box-title-toolbar">
                <ul>
                  <li>
                    <% if (division.CanDelete) { 
                         using (var form = Html.BeginForm<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.DeleteDivision(division.Id), FormMethod.Post)) { %>
                    <input type="image" value="Delete" alt="Delete" src="<%= Url.ContentImageUrl("delete-small.png")%>"/>
                    <%= Html.AntiForgeryToken()%>
                    <%   }
                       } %>
                    <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.AddTeamToDivision(division.Id)) %>">
                      <%= Html.ContentImage("add-small.png", "Add Team") %>
                    </a>
                  </li>
                </ul>
              </div>
            </div>
            <div class="content-box-content">content</div>
          </div>
      <% } %>
    <% } %>
    </div>
    <div class="action-button-row">
      <div class="action-button">
        <%= Html.ContentImage("add-medium.png", "Add Division") %>
        <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.AddDivision(Model.Id), "Add a new division to this season") %>
      </div>
    </div>
  </div>
  <%
    if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
      Html.RenderPartial("ErrorMessage");
    }
    else if (TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage)) {
      Html.RenderPartial("NotificationMessage");
    }
  %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
