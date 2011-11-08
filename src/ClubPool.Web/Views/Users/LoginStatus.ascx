<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Users.ViewModels.LoginStatusViewModel>" %>
<% if (Model.UserIsLoggedIn) { %>
<%= Html.Encode(Model.Username)%> <%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(x => x.Logout(), "logout")%>
<% } else { %>
not logged in <%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(x => x.Login(string.Empty), "login")%>
<% } %>