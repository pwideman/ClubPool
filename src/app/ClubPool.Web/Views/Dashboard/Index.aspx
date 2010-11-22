<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <%= Html.ContentImage("home.png", "Home") %>
    <span><%= Model.UserFullName %>'s Dashboard</span>
  </div>
  <div>
    <div>Current Season Stats & Info</div>
    <ul>
      <li>Skill Level: <%= Model.CurrentSeasonStats.SkillLevel %></li>
      <li>Team Name: <%= Model.CurrentSeasonStats.TeamName %></li>
      <li>Teammate: <%= Model.CurrentSeasonStats.Teammate %></li>
      <li>Team Record: <%= Model.CurrentSeasonStats.TeamRecord %></li>
      <li>My Record: <%= Model.CurrentSeasonStats.PersonalRecord %></li>
    </ul>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
  Dashboard
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
