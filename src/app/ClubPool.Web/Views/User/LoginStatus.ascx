<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.User.ViewModels.LoginStatusViewModel>" %>
<%
if (Model.UserIsLoggedIn) { 
  Response.Write(Model.Username + " " + Html.ActionLink<ClubPool.Web.Controllers.UserController>(x => x.Logout(), "logout"));
} else {
  Response.Write(Html.ActionLink<ClubPool.Web.Controllers.UserController>(x => x.Login(string.Empty), "login"));
}
%>