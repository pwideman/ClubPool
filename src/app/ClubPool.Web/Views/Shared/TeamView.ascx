<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Core.TeamDto>" %>
<div class="content-box" style="display: inline-block; min-width: 100px;">
  <div class="content-box-title">
    <span class="content-box-title-heading">
      <%= Html.ActionLink<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Edit(Model.Id), Model.Name) %>
    </span>
    <div class="content-box-title-toolbar">
      <ul>
        <li>
          <% if (Model.CanDelete) { 
               using (var form = Html.BeginForm<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Delete(Model.Id), FormMethod.Post)) { %>
          <input type="image" value="Delete" alt="Delete" src="<%= Url.ContentImageUrl("delete-small.png")%>"/>
          <%= Html.AntiForgeryToken()%>
          <%   }
             } %>
        </li>
      </ul>
    </div>
  </div>
  <div class="content-box-content team-view-content">
    <% foreach (var player in Model.Players) { %>
      <%= player.FullName %>
    <% } %>
  </div>
</div>
