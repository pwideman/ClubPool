<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Seasons.ViewModels.TeamViewModel>" %>
<div class="content-box" style="display: inline-block;">
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
  <div class="team-view-content">
    <% if (Model.Players.Any()) { %>
      <ul>
      <% foreach (var player in Model.Players) { %>
        <li><%= player.Name%></li>
      <% } %>
      </ul>
      <% } else { %>
       No Players
      <% } %>
  </div>
</div>
