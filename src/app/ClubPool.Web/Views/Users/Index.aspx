<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IPagination<ClubPool.Core.UserDto>>" %>
<%@ Import Namespace="MvcContrib.UI.Pager"%>
<%@ Import Namespace="MvcContrib.UI.Grid"%>
<%@ Import Namespace="MvcContrib.Pagination"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <%= Html.ContentImage("users.png", "Users") %>
  <h3 class="heading">Users</h3>
  <div>
    <div style="display:inline-block; clear: both;">
	    <%= Html.Grid(Model).Columns(column => {
            column.For(x => x.Id);
     		    column.For(x => x.Username);
     		    column.For(x => x.FullName).Named("Name");
            column.For(x => x.Email);
            column.For(x => x.IsApproved ? "Yes" : "No").Named("Approved");
            column.For(x => string.Join(", ", x.Roles)).Named("Roles");
            column.For(x => @"<a href=""" +
              Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(u => u.Edit(x.Id)) +
              @""">" + Html.ContentImage("edit.png", "Edit") + @"</a>&nbsp;<a href=""" +
              Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(u => u.Delete(x.Id)) +
              @""">" + Html.ContentImage("delete.png", "Delete") + "</a>").Encode(false);
     	    }) %>
      <div class="pager"><%= Html.Pager(Model) %></div>
    </div>
  </div>
  <p>
    <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(u => u.Create()) %>">
      <div class="blue-button">
        <%= Html.ContentImage("add.png", "Add") %>
        Add a new user
      </div>
    </a>
  </p>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Users - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>

