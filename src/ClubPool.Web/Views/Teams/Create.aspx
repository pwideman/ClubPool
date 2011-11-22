<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Teams.ViewModels.CreateTeamViewModel>" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Add team to <%= Html.Encode(Model.DivisionName) %></span>
  </div>
  <div class="form-content">
    <div class="form-header">
      All fields are required
    </div>
    <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
         Html.RenderPartial("ErrorMessage");
       } 
       using (var form = Html.BeginForm<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Create(null), FormMethod.Post, new { @class = "normal" })) {
    %>
    <fieldset>
      <%= Html.AntiForgeryToken()%>
      <%= Html.HiddenFor(m => m.DivisionId) %>
      <% Html.RenderPartial("TeamEditControl"); %>
      <div class="spacer">&nbsp;</div>
      <div class="form-row-span">
        <input class="submit-button" type="submit" value="Create Team" />
      </div>
    </fieldset>
    <% } %>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Add Team
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<%= Html.ScriptInclude("jquery.alphanumeric.js") %>
</asp:Content>
