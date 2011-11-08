<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.UnapprovedViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <%= Html.ContentImage("unapproveduser-large.png", "Unapproved Users") %>
    <span>Users Awaiting Approval</span>
  </div>
  <% if (Model.UnapprovedUsers.Any()) { %>
  <p class="heading">The following users have signed up and are awaiting approval. Approve them to give them access to the website.</p>
  <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Approve(null))) { %>
    <%= Html.AntiForgeryToken() %>
    <table style="width: 600px;" class="domain-list" id="unapproved_users">
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th><input type="checkbox" id="approveall" /></th>
        </tr>
      </thead>
      <tfoot>
        <tr>
          <td colspan="2" />
          <td align="center"><input class="submit-button" type="submit" value="Approve" /></td>
        </tr>
      </tfoot>
      <tbody class="content">
        <% foreach (var user in Model.UnapprovedUsers) { %>
          <tr>
            <td><%= Html.Encode(user.Name) %></td>
            <td><%= Html.Encode(user.Email) %></td>
            <td><input type="checkbox" value="<%= user.Id%>" name="userids" /></td>
          </tr>
        <% } %>
      </tbody>
    </table>
  <% } %>
  <%} else { %>
  <p>There are no users awaiting approval.</p>
  <% } 
    if (TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage)) {
      Html.RenderPartial("NotificationMessage");
    }
    if (TempData.ContainsKey("FailedEmails")) {
      var failedEmails = TempData["FailedEmails"] as List<ClubPool.Core.User>; %>
    <p>The system failed to send email notification to the following users:
      <ul>
        <% foreach(var user in failedEmails) { %>
        <li><%= Html.Encode(string.Format("{0} ({1})", user.FullName, user.Email)) %></li>
        <% } %>
      </ul>
    </p>
 <% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">Users Awaiting Approval
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
  <script type="text/javascript">
    $(function () {
      $("#approveall").click(function () {
        var approved = $(this).attr("checked");
        $("input[name=userids]").attr("checked", approved);
      });
      $("#unapproved_users tbody.content tr:odd").addClass("alt");
    });
  </script>
</asp:Content>