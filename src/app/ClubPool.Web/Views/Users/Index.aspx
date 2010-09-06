<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.IndexViewModel>" %>
<%@ Import Namespace="MvcContrib.UI.Pager"%>
<%@ Import Namespace="MvcContrib.UI.Grid"%>
<%@ Import Namespace="MvcContrib.Pagination"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <%= Html.ContentImage("users-large.png", "Users") %>
    <span>Users</span>
  </div>
  <div class="action-button-row">
    <div class="action-button">
      <%= Html.ContentImage("adduser-medium.png", "Add a new user") %>
      <%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(u => u.Create(), "Add a new user") %>
    </div>
    <div class="action-button">
      <%= Html.ContentImage("unapproveduser-medium.png", "Unapproved users") %>
      <%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(u => u.Unapproved(), "Unapproved users") %>
    </div>
  </div>
  <div>
    <table class="domain-list">
      <thead>
        <tr>
          <th>Id</th>
          <th>Username</th>
          <th>Name</th>
          <th>Email</th>
          <th>Status</th>
          <th>Roles</th>
          <th></th>
          <th></th>
        </tr>
      </thead>
      <tbody>
    <% foreach (var item in Model.Items) { %>
        <tr>
          <td><%= Html.Encode(item.Id) %></td>
          <td><%= Html.Encode(item.Username) %></td>
          <td><%= Html.Encode(item.Name) %></td>
          <td><%= Html.Encode(item.Email) %></td>
          <td>
            <% if (!item.IsApproved) { %>
            <%= Html.ContentImage("block-medium.png", "Unapproved")%>
            <% }
              if (item.IsLocked) { %>
            <%= Html.ContentImage("lock-medium.png", "Locked")%>
            <% } %>
          </td>
          <td><%= string.Join(", ", item.Roles) %></td>
          <td class="action-column">
            <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(c => c.Edit(item.Id)) %>">
            <%= Html.ContentImage("edit-medium.png", "Edit") %>
            </a>
          </td>
          <td class="action-column">
            <% if (item.CanDelete) {
                 using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Delete(item.Id, Model.CurrentPage), FormMethod.Post)) { %>
              <input type="image" value="Delete" alt="Delete" src="<%= Url.ContentImageUrl("delete-medium.png")%>"/>
              <%= Html.AntiForgeryToken()%>
            <%   }
               } %>
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
Users - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>

