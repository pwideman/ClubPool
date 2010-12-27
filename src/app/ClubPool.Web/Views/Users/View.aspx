﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.ViewViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h4>User Information</h4>

<div id="tabs" class="userinfo-tabs">
  <ul>
    <li><a href="#details-tab">Details</a></li>
    <li><a href="#history-tab">History</a></li>
  </ul>

  <div id="details-tab" class="userinfo-tab">
    <div class="details-row">
      <div class="details-label">Username:</div>
      <div class="details-data"><%= Html.Encode(Model.Username) %></div>
    </div>
    <div class="details-row">
      <div class="details-label">Name:</div>
      <div class="details-data"><%= Html.Encode(Model.Name)%></div>
    </div>
    <div class="details-row">
      <div class="details-label">Email:</div>
      <div class="details-data"><%= Html.ActionLink<ClubPool.Web.Controllers.Contact.ContactController>(c => c.Player(Model.Id), Model.Email)%></div>
    </div>
    <div class="details-row">
      <div class="details-label">8-ball skill level:</div>
      <div class="details-data"><%= Model.SkillLevel > 0 ? Model.SkillLevel.ToString() : "None" %></div>
    </div>
    <% if (Model.ShowAdminProperties) { %>
    <fieldset>
      <legend>Admin</legend>
      <div>Roles: <%= Model.Roles.Length > 0 ? Html.Encode(string.Join(", ", Model.Roles)) : "None"%></div>
      <div>User is <%= Model.IsApproved ? "approved" : "unapproved"%></div>
      <div>User is <%= Model.IsLocked ? "locked" : "unlocked"%></div>
    </fieldset>
    <% } %>
  </div>

  <div id="history-tab" class="userinfo-tab">
    Coming soon...
  </div>
</div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
User Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<script type="text/javascript">
  $(document).ready(function () {
    $("#tabs").tabs();
  });
</script>
</asp:Content>
