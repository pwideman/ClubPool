<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Users.ViewModels.LoginStatusViewModel>" %>
<% if (Model.UserIsLoggedIn) { %>
<%= Html.Encode(Model.Username)%> <%= Html.ActionLink("logout", "Logout", "Users")%>
<% } else { %>
not logged in <%= Html.ActionLink("login", "Login", "Users")%>
<% } %>