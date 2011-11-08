﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.IndexViewModel>" %>
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
    <div class="users-search-container">
      <input value="<%= Html.Encode(Model.SearchQuery) %>" type="text" name="q" id="q" placeholder="Search" title="Enter all or part of a username, first or last name to search" class="users-search-input"/>
      <%= Html.ContentImage("search-medium.png", "Search", new { id = "users-search-icon", @class = "users-search-icon" })%>
    </div>
    <div class="users-list-container">
      <% if (Model.Items.Any()) { %>
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
        <tbody class="content mouseover-highlight-row">
      <% foreach (var item in Model.Items) { %>
          <tr>
            <td><%= item.Id%></td>
            <td><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.View(item.Id), item.Username)%></td>
            <td><%= Html.Encode(item.Name)%></td>
            <td><%= Html.Encode(item.Email)%></td>
            <td>
              <% if (!item.IsApproved) { %>
              <%= Html.ContentImage("block-medium.png", "Unapproved")%>
              <% }
                 if (item.IsLocked) { %>
              <%= Html.ContentImage("lock-medium.png", "Locked")%>
              <% } %>
            </td>
            <td><%= string.Join(", ", item.Roles)%></td>
            <td class="action-column">
              <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(c => c.Edit(item.Id)) %>">
              <%= Html.ContentImage("edit-medium.png", "Edit")%>
              </a>
            </td>
            <td class="action-column">
              <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Delete(item.Id, Model.CurrentPage, Model.SearchQuery), FormMethod.Post)) { %>
                <input type="image" value="Delete" alt="Delete" src="<%= Url.ContentImageUrl("delete-medium.png")%>"/>
                <%= Html.AntiForgeryToken()%>
              <% } %>
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
      <% } else if (!string.IsNullOrEmpty(Model.SearchQuery)) { %>
      <p>There are no users matching your search terms.</p>
      <% } else { %>
      <p>There are no users.</p>
      <% } %>
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

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">Users</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<%= Html.Script("jquery.placeholder.js") %>
<%= Html.Script("jquery.query.js") %>

<script type="text/javascript">
  var prevQuery = "";
  $(function () {
    <% if (!string.IsNullOrEmpty(Model.SearchQuery)) { %>
    prevQuery = "<%= Model.SearchQuery %>";
    <% } %>
    $("tbody.content tr:odd").addClass("alt");
    $("#q").placeholder().keypress(function (e) {
      if (e.keyCode === 13) {
        doSearch($(this).val());
      }
    });
    $("#users-search-icon").click(function(e) {
      doSearch($("#q").val());
    });
  });

  function doSearch(query) {
    if (query !== prevQuery) {
      var search = "";
      if (query) {
        search = $.query.set("q", query).remove("page");
      }
      else {
        search = $.query.remove("q").remove("page");
      }
      var newurl = location.origin + location.pathname + search;
      location.assign(newurl);
    }
  }
</script>
</asp:Content>
