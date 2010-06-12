<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.CreateViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="formContainer">
    <div class="formTitle">Add User</div>
    <div class="formContent">
      <div class="formHeader">
        All fields are required
      </div>
      <% if (!string.IsNullOrEmpty(Model.ErrorMessage)) {
           Html.RenderPartial("FormError");
         } 
         using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Create(null), FormMethod.Post, new { @class = "normal" })) {
      %>
      <fieldset>
        <%= Html.AntiForgeryToken()%>
        <%= Html.HiddenFor(m => m.PreviousUsername)%>
        <% Html.RenderPartial("CreateUserControl"); %>
        <div class="spacer">&nbsp;</div>
        <div class="formrowspan">
          <input class="submit-button" type="submit" value="Create User" />
        </div>
      </fieldset>
      <% } %>
      <%= Html.ClientSideValidation<ClubPool.Web.Controllers.Users.ViewModels.CreateViewModel>() %>
    </div>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>

