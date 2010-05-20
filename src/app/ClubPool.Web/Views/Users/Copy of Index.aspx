<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<ClubPool.Core.UserDto>>" %>
<%@ Import Namespace="MvcContrib.UI.Pager"%>
<%@ Import Namespace="MvcContrib.UI.Grid"%>
<%@ Import Namespace="MvcContrib.Pagination"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <%= Html.ContentImage("users.png", "Users") %>
    <h3 class="heading">Users</h3>
    <table>
      <thead>
        <tr>
          <th>Id</th>
          <th>Username</th>
          <th>Name</th>
          <th>Email</th>
          <th>Approved</th>
          <th>Roles</th>
          <th></th>
        </tr>
      </thead>
      <tfoot>
        <tr>
          <td colspan="2">
            <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(u => u.Create()) %>">
              <div class="blue-button">
                <%= Html.ContentImage("add.png", "Add") %>
                Add a new user
              </div>
            </a>
          </td>
          <td colspan="99" class="submit-row">
            <span>
              << < Page > >>
            </span>
          </td>
        </tr>
      </tfoot>
      <tbody>
    <% foreach (var item in Model) { %>
        <tr>
          <td><%= Html.Encode(item.Id) %></td>
          <td><%= Html.Encode(item.Username) %></td>
          <td><%= Html.Encode(item.FullName) %></td>
          <td><%= Html.Encode(item.Email) %></td>
          <td><%= item.IsApproved ? "Yes" : "No" %></td>
          <td>
            <%= string.Join(", ", item.Roles) %>
          </td>
          <td>
            <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(u => u.Edit(item.Id)) %>">
            <%= Html.ContentImage("edit.png", "Edit") %>
            </a>
            <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Users.UsersController>(u => u.Delete(item.Id)) %>">
              <%= Html.ContentImage("delete.png", "Delete") %>
            </a>
          </td>
        </tr>
    <% } %>
      </tbody>
    </table>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Users - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>

