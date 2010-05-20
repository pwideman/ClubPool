<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.UnapprovedViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <h4>Users Awaiting Approval</h4>
  <% if (Model.UnapprovedUsers.Count() > 0) { %>
  <p class="heading">The following users have signed up and are awaiting approval. Approve them to give them access to the website.</p>
  <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Approve(null))) { %>
    <%= Html.AntiForgeryToken() %>
    <table style="width: 600px">
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Approve</th>
        </tr>
      </thead>
      <tfoot>
        <tr>
          <td colspan="2" />
          <td align="center"><button type="submit">Approve</button></td>
        </tr>
      </tfoot>
      <tbody>
        <% foreach (var user in Model.UnapprovedUsers) { %>
          <tr>
            <td><%= Html.Encode(user.FullName) %></td>
            <td><%= Html.Encode(user.Email) %></td>
            <td><input type="checkbox" value="<%= user.Id%>" name="userids" /></td>
          </tr>
        <% } %>
      </tbody>
    </table>
  <% } %>
  <%} else { %>
  <p>There are no users awaiting approval.</p>
  <% } %>
  <% if (TempData.ContainsKey("message")) { %>
		<div class="ui-state-highlight ui-corner-all notification"> 
			<p><span class="ui-icon ui-icon-info notification-icon"></span>
			<%= Html.Encode(TempData["message"]) %></p>
		</div>
  <% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">Users Awaiting Approval
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
