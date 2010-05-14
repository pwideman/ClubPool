<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="formContainer corner">
    <div class="formTitle">Login</div>
    <div class="formContent corner">
      <div class="spacer">&nbsp;</div>
      <form method="post" id="loginForm" class="loginForm normal" action="<%= Html.BuildUrlFromExpressionForAreas<ClubPool.Web.Controllers.UsersController>(c => c.Login(string.Empty))%>">
        <% Html.RenderPartial("LoginControl"); %>
      </form>
    </div>
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
