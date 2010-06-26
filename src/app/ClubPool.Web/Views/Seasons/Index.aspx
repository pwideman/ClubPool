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
  </div>
  <div>
    <table style="width: 500px;">
      <thead>
        <tr>
          <th>Id</th>
          <th>Name</th>
          <th></th>
          <th></th>
        </tr>
      </thead>
      <tbody>
    <% foreach (var item in Model.Seasons) { %>
        <tr>
          <td><%= Html.Encode(item.Id) %></td>
          <td><%= Html.Encode(item.Name) %></td>
          <td class="action-column">
            <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Edit(item.Id)) %>">
            <%= Html.ContentImage("edit-medium.png", "Edit") %>
            </a>
          </td>
          <td class="action-column">
            <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Delete(item.Id, Model.CurrentPage), FormMethod.Post, new { @class = "normal" })) { %>
              <input type="image" value="Delete" alt="Delete" src="<%= Url.ContentImageUrl("delete-medium.png")%>"/>
              <%= Html.AntiForgeryToken() %>
            <% } %>
          </td>
        </tr>
    <% } %>
      <tr class="pager">
        <td colspan="99">
          <% Html.RenderPartial("Pager"); %>
        </td>
      </tr>
      </tbody>
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
