<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.User.ViewModels.LoginStatusViewModel>" %>
<% if (Model.IsLoggedIn) { %>
<%= Model.Username %> <%= Html.ActionLink<ClubPool.Web.Controllers.User.UserController>(x => x.Logout(), "logout") %>
<% } else { %>
<%= Html.ActionLink<ClubPool.Web.Controllers.User.UserController>(x => x.Login(string.Empty), "login") %>
<% } %>