<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.User.ViewModels.LoginViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div id="loginFormContainer">
    <form method="post" id="loginForm" class="loginForm normal normalRoundCorners" action="<%= Html.BuildUrlFromExpressionForAreas<ClubPool.Web.Controllers.UserController>(c => c.Login(string.Empty))%>">
      <div class="formTitle">Login</div>
      <div class="formContent normalRoundCorners">
        <% Html.RenderPartial("LoginControl"); %>
      </div>
    </form>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Login
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<script type="text/javascript">
  $(document).ready(function() {
    $("#loginForm").validate({
      errorPlacement: function() { /* don't show error labels, just highlight*/ }
    });
  });
</script>
</asp:Content>
