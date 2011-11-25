<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Teams.ViewModels.EditTeamViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Edit team <%= Html.Encode(Model.Name) %></span>
  </div>
  <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
       Html.RenderPartial("ErrorMessage");
     } %>
  <div class="form-content">
    <div class="form-header">
      All fields are required
    </div>
    <% using (var form = Html.BeginForm("Edit", "Teams", FormMethod.Post, new { @class = "normal" })) { %>
    <fieldset>
      <%= Html.AntiForgeryToken()%>
      <%= Html.HiddenFor(m => m.Id) %>
      <%= Html.HiddenFor(m => m.Version) %>
      <% Html.RenderPartial("TeamEditControl"); %>
      <div class="spacer">&nbsp;</div>
      <div class="form-row-span">
        <input class="submit-button" type="submit" value="Save" />
      </div>
    </fieldset>
    <% } %>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Edit Team
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<%= Html.ScriptInclude("jquery.alphanumeric.js") %>
</asp:Content>
