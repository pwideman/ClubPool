<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.IndexViewModel>" %>
<%@ Import Namespace="MvcContrib.UI.Pager"%>
<%@ Import Namespace="MvcContrib.UI.Grid"%>
<%@ Import Namespace="MvcContrib.Pagination"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div>
    <%= Html.ContentImage("users.png", "Users") %>
    <h3 class="heading">Users</h3>
  </div>
  <div class="action-button-row">
    <div class="action-button">
      <%= Html.ContentImage("add.png", "Add a new user") %>
      <%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(u => u.Create(), "Add a new user") %>
    </div>
  </div>
  <div>
    <div style="display:inline-block; clear: both;">
      <%= Html.Grid(Model.Users).Columns(column => {
            column.For(x => x.Id);
            column.For(x => x.Username);
            column.For(x => x.FullName).Named("Name");
            column.For(x => x.Email);
            column.For(x => x.IsApproved ? "Yes" : "No").Named("Approved");
            column.For(x => string.Join(", ", x.Roles)).Named("Roles");
            column.For(x => @"<a href=""" +
              Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(u => u.Edit(x.Id)) +
              @""">" + Html.ContentImage("edit.png", "Edit") + @"</a>").Encode(false);
            column.For(x => @"<form action=""" +
              Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(u => u.Delete(x.Id, Model.Page)) +
              @""" method=""post""><input type=""image"" value=""Delete"" alt=""Delete"" src=""" + Url.ContentImageUrl("delete.png") + @"""/>" +
              Html.AntiForgeryToken() + "</form>").Encode(false);
          }) %>
      <div class="pager"><%= Html.Pager(Model.Users) %></div>
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
Users - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>

