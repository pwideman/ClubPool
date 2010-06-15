<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.EditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div>
    <%= Html.ContentImage("users.png", "Users") %>
    <h3 class="heading">Edit User</h3>
  </div>
  <div class="form-content">
    <div class="form-header">
      All fields are required
    </div>
    <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
          Html.RenderPartial("ErrorMessage");
        } %>
    <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Edit(null), FormMethod.Post, new { @class = "normal" })) { %>
      <fieldset>
        <%= Html.AntiForgeryToken()%>
        <%= Html.HiddenFor(m => m.Id) %>
        <div class="form-row">
          <%= Html.LabelFor(m => m.Username)%>
          <div class="form-input">
            <%= Html.TextBoxFor(m => m.Username)%>
            <%= Html.ValidationMessageFor(m => m.Username)%>
          </div>
        </div>
        <div class="form-row">
          <%= Html.LabelFor(m => m.Email)%>
          <div class="form-input">
            <%= Html.TextBoxFor(m => m.Email)%>
            <%= Html.ValidationMessageFor(m => m.Email)%>
          </div>
        </div>
        <div class="form-row">
          <label for="FirstName">Name:</label>
          <div class="form-input">
            <%= Html.TextBoxFor(m => m.FirstName, new { @class = "short" })%>
            <%= Html.LabelFor(m => m.FirstName)%>
            <%= Html.ValidationMessageFor(m => m.FirstName)%>
          </div>
          <div class="form-input">
            <%= Html.TextBoxFor(m => m.LastName, new { @class = "short" })%>
            <%= Html.LabelFor(m => m.LastName)%>
            <%= Html.ValidationMessageFor(m => m.LastName)%>
          </div>
        </div>
        <div class="form-row">
          <%= Html.LabelFor(m => m.IsApproved) %>
          <%= Html.CheckBoxFor(m => m.IsApproved, new { style = "vertical-align: bottom;" })%>
        </div>
        <div class="spacer">&nbsp;</div>
        <div class="form-row-span">
          <input class="submit-button" type="submit" value="Save" />
        </div>
      </fieldset>
    <% } %>
    <%= Html.ClientSideValidation<ClubPool.Web.Controllers.Users.ViewModels.EditViewModel>() %>
  </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Edit User - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
