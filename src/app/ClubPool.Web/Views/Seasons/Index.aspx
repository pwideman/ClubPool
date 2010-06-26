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
          <td class="action-column">
            <a href="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Edit(item.Id)) %>">
            <%= Html.ContentImage("edit-medium.png", "Edit") %>
            </a>
          </td>
          <td class="action-column">
            <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Delete(item.Id, Model.CurrentPage), FormMethod.Post, new { @class = "normal" })) { %>
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
          <% if (Model.CurrentPage > 1) { %>
          <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(1), "<<")%>
          <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(Model.CurrentPage - 1), "<")%>
          <% } else { %>
          << <
          <% }
            if (Model.TotalPages > 2) {
              var numPageLinksToShow = 7;
              var first = Math.Max(Model.CurrentPage - numPageLinksToShow / 2, 1);
              var last = Math.Min(Model.CurrentPage + numPageLinksToShow / 2, Model.TotalPages);
              if (last - first < numPageLinksToShow - 1) {
                if (1 == first) {
                  last += numPageLinksToShow - (last - first) - 1;
                }
                else {
                  first -= numPageLinksToShow - (last - first) - 1;
                }
                last = Math.Min(last, Model.TotalPages);
                first = Math.Max(first, 1);
              }
              for (int i = first; i < last+1; i++) {
                if (i != Model.CurrentPage) {
                  Response.Write(@"<a href=""?page=" + i.ToString() + @""">" + i.ToString() + "</a>\n");
                  //Response.Write(Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(i), i.ToString()) + "\n");
                }
                else {
                  Response.Write(i.ToString() + "\n");
                }
              }
            }
          %>
          <%  if (Model.CurrentPage < Model.TotalPages) { %>
          <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(Model.CurrentPage + 1), ">")%>
          <%= Html.ActionLink<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Index(Model.TotalPages), ">>")%>
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

