﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Seasons.ViewModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Seasons</span>
  </div>
  <div class="action-button-row">
    <div class="action-button">
      <%= Html.ContentImage("add-medium.png", "Add a new season") %>
      <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Create(), "Add a new season") %>
    </div>
    <div class="action-button">
      <%= Html.ContentImage("check-medium.png", "Change active season") %>
      <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.ChangeActive(), "Change the active season") %>
    </div>
  </div>
  <div>
    <table class="domain-list" style="width: 500px;">
      <thead>
        <tr>
          <th>Id</th>
          <th>Name</th>
          <th>Active</th>
          <th></th>
          <th></th>
        </tr>
      </thead>
      <tbody class="content mouseover-highlight-row">
    <% foreach (var item in Model.Items) { %>
        <tr>
          <td><%= Html.Encode(item.Id) %></td>
          <td><%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.View(item.Id), item.Name) %></td>
          <td>
            <% if (item.IsActive) { %>
            <%= Html.ContentImage("check-medium.png", "Current")%>
            <% } %>
          </td>
          <td class="action-column">
            <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Edit(item.Id)) %>">
            <%= Html.ContentImage("edit-medium.png", "Edit") %>
            </a>
          </td>
          <td class="action-column">
            <% if (item.CanDelete) {
                 using (var form = Html.BeginForm<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Delete(item.Id, Model.CurrentPage), FormMethod.Post)) { %>
              <input type="image" value="Delete" alt="Delete" src="<%= Url.ContentImageUrl("delete-medium.png")%>"/>
              <%= Html.AntiForgeryToken()%>
            <%   }
               } %>
          </td>
        </tr>
    <% } %>
      </tbody>
      <tr class="pager">
        <td colspan="99">
          <% Html.RenderPartial("Pager"); %>
        </td>
      </tr>
    </table>
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
Seasons - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>

