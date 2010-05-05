<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ClubPool.Web.Views.AspxViewPageBase<ClubPool.Web.Controllers.User.ViewModels.UnapprovedViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <% if (Model.UnapprovedUsers.Count > 0) { %>
  <h2>Users awaiting approval</h2>
  <table>
    <thead>
      <tr>
        <th>Name</th>
        <th>Email</th>
        <th>Approve</th>
      </tr>
    </thead>
    <tbody>
      <% foreach (var user in Model.UnapprovedUsers) { %>
        <tr>
          <td><%= Html.Encode(user.FullName) %></td>
          <td><%= Html.Encode(user.Email) %></td>
          <td><input type="checkbox" id="<%= user.Id%>" name="<%= user.Id%>" /></td>
        </tr>
      <% } %>
    </tbody>
  </table>
  <%} else { %>
  <p>There are no users awaiting approval.</p>
  <% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">Users Awaiting Approval
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
