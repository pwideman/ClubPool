<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.EditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <%= Html.ContentImage("edituser-large.png", "Edit User") %>
    <span>Edit User</span>
  </div>
  <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
        Html.RenderPartial("ErrorMessage");
      } %>
  <div class="form-content edit-user-form">
    <div class="form-header">
      All fields are required
    </div>
    <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Edit(null), FormMethod.Post, new { @class = "normal" })) { %>
      <fieldset>
        <%= Html.AntiForgeryToken()%>
        <%= Html.HiddenFor(m => m.Id) %>
        <%= Html.HiddenFor(m => m.Version) %>
        <div class="form-row">
          <span class="form-label-left"><%= Html.LabelFor(m => m.Username)%></span>
          <div class="form-input">
            <%= Html.TextBoxFor(m => m.Username)%>
            <%= Html.ValidationMessageFor(m => m.Username)%>
          </div>
        </div>
        <div class="form-row">
          <span class="form-label-left"><%= Html.LabelFor(m => m.Email)%></span>
          <div class="form-input">
            <%= Html.TextBoxFor(m => m.Email)%>
            <%= Html.ValidationMessageFor(m => m.Email)%>
          </div>
        </div>
        <div class="form-row">
          <span class="form-label-left"><label for="FirstName">Name:</label></span>
          <div class="form-input">
            <%= Html.TextBoxFor(m => m.FirstName, new { @class = "short" })%>
            <span class="form-sublabel"><%= Html.LabelFor(m => m.FirstName)%></span>
            <%= Html.ValidationMessageFor(m => m.FirstName)%>
          </div>
          <div class="form-input">
            <%= Html.TextBoxFor(m => m.LastName, new { @class = "short" })%>
            <span class="form-sublabel"><%= Html.LabelFor(m => m.LastName)%></span>
            <%= Html.ValidationMessageFor(m => m.LastName)%>
          </div>
        </div>
        <div class="form-row form-row-short">
          <span class="form-label-left">Status:</span>
          <div class="form-checkbox-list">
            <ul>
              <li>
                <%= Html.CheckBoxFor(m => m.IsApproved)%>
                <%= Html.LabelFor(m => m.IsApproved)%>
              </li>
              <li>
                <%= Html.CheckBoxFor(m => m.IsLocked)%>
                <%= Html.LabelFor(m => m.IsLocked)%>
              </li>
            </ul>
          </div>
        </div>
        <div class="form-row">
          <span class="form-label-left"><%= Html.LabelFor(m => m.AvailableRoles) %></span>
          <div class="form-checkbox-list">
            <ul>
              <% foreach (var role in Model.AvailableRoles) { %>
              <li>
                <input type="checkbox" name="Roles" id="<%= "Role" + role.Id.ToString() %>" value="<%= role.Id %>" <%
                  if (Model.Roles.Contains(role.Id)) {
                    %>checked="checked"<%
                  } %> />
                <label for="<%= "Role" + role.Id.ToString() %>"><%= role.Name%></label>
              </li>
              <% } %>
            </ul>
          </div>
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
