<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Seasons.ViewModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Seasons</span>
  </div>
  <div class="action-button-row">
    <div class="action-button">
      <%= Html.ContentImage("add-medium.png", "Add a new season") %>
      <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Create(), "Add a new season") %>
    </div>
  </div>
  <div>
    <table style="width: 600px;">
      <thead>
        <tr>
          <th>Id</th>
          <th>Name</th>
          <th></th>
          <th></th>
        </tr>
      </thead>
      <tbody>
    <% foreach (var item in Model.Seasons) { %>
        <tr>
          <td><%= Html.Encode(item.Id) %></td>
          <td><%= Html.Encode(item.Name) %></td>
          <td>
            <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Edit(item.Id)) %>">
            <%= Html.ContentImage("edit-medium.png", "Edit") %>
            </a>
          </td>
          <td>
            <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Delete(item.Id, Model.Page), FormMethod.Post, new { @class = "normal" })) { %>
              <input type="image" value="Delete" alt="Delete" src="<%= Url.ContentImageUrl("delete-medium.png")%>"/>
              <%= Html.AntiForgeryToken() %>
            <% } %>
          </td>
        </tr>
    <% } %>
      <tr class="pager">
        <td colspan="99">
          <span class="pager-info">Showing <%= Model.First %> - <%= Model.Last %> of <%= Model.Total %></span>
          <span class="pager-links">
          <% if (Model.Page > 1) { %>
          <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(1), "<<")%>
          <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(Model.Page - 1), "<")%>
          <% } else { %>
          << <
          <% }
            if (Model.LastPage > 2) {
              var first = Math.Max(Model.Page - 8, 1);
              for (int i = first; i < Model.Page; i++) {
                %>
                <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(i), i.ToString()) %>
                <%
              }
              %>
              <%= Model.Page.ToString() %>
              <%
              var last = Math.Min(Model.Page + (8 - (Model.Page - first)), Model.LastPage);
              for (int i = Model.Page + 1; i <= last; i++) {
                %>
                <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(i), i.ToString()) %>
                <%
              }
            }
          %>
          <%  if (Model.Page < Model.LastPage) { %>
          <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(Model.Page + 1), ">")%>
          <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(Model.LastPage), ">>")%>
          <% } else { %>
          > >>
          <% } %>
          </span>
        </td>
      </tr>
      </tbody>
    </table>
  </div>
  <%
    if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
      Html.RenderPartial("ErrorMessage");
    }
    else if (TempData.ContainsKey(GlobalViewDataProperty.PageNotificationMessage)) {
      Html.RenderPartial("NotificationMessage");
    }
  %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Seasons - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>

