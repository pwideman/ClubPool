<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <% if (Model.UserIsAdmin) {
       if (Model.NewUsersAwaitingApproval.Count() > 0) { %>
         <table>
         <%
         foreach(var user in Model.NewUsersAwaitingApproval) {
           Response.Write("<tr><td>" + user.Username + "</td><td>" + user.FullName + "</td><td>" + user.Email + "</td></tr>");
         }
         %>
         </table>
      <% }     
  
  } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
  Dashboard
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
