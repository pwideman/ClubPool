<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Users.ViewModels.LoginStatusViewModel>" %>
<%
if (Model.UserIsLoggedIn) { 
  Response.Write(Model.Username + " " + Html.ActionLink<ClubPool.Web.Controllers.UsersController>(x => x.Logout(), "logout"));
} else {
  Response.Write("not logged in " + Html.ActionLink<ClubPool.Web.Controllers.UsersController>(x => x.Login(string.Empty), "login"));
}
%>