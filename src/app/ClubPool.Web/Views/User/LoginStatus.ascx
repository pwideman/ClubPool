<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.User.ViewModels.LoginStatusViewModel>" %>
<%
if (Model.IsLoggedIn) { 
  Response.Write(Model.Username + " " + Html.ActionLink<ClubPool.Web.Controllers.User.UserController>(x => x.Logout(), "logout"));
} else {
  Response.Write(Html.ActionLink<ClubPool.Web.Controllers.User.UserController>(x => x.Login(string.Empty), "login"));
}
%>