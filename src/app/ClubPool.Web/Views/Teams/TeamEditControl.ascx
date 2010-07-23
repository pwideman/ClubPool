<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Teams.ViewModels.TeamViewModel>" %>
<%= Html.HiddenFor(m => m.Division.Id) %>
<div class="form-row">
  <span class="form-label-left"><%= Html.LabelFor(m => m.Name)%></span>
  <div class="form-input">
    <%= Html.TextBoxFor(m => m.Name)%>
    <%= Html.ValidationMessageFor(m => m.Name)%>
  </div>
</div>
<div class="form-row">
  <span class="form-label-left"><%= Html.LabelFor(m => m.Players) %></span>
  <span id="PlayersList">
    <% Html.RenderPartial("SimpleUserListView", Model.Players); %>
  </span>
</div>
<div class="form-row" style="margin: 10px;padding:0; min-height:0">
  <span class="form-label-left">&nbsp;</span>
  <div class="form-input">Drag users to/from Players and Available Players</div>
</div>
<div class="form-row">
  <span class="form-label-left"><%= Html.LabelFor(m => m.AvailablePlayers) %></span>
  <span id="AvailablePlayersList">
    <% Html.RenderPartial("SimpleUserListView", Model.AvailablePlayers); %>
  </span>
</div>
<script type="text/javascript">
  var playersList, availablePlayersList;

  $(document).ready(function () {
    playersList = $("#PlayersList > div");
    availablePlayersList = $("#AvailablePlayersList > div");
    playersList.droppable({
      drop: function (event, ui) {
        $(this).append(ui.draggable);
      }
    });
    availablePlayersList.droppable({
      drop: function (event, ui) {
        $(this).append(ui.draggable);
      }
    });
    playersList.children().each(function () {
      $(this).draggable();
    });
    availablePlayersList.children().each(function () {
      $(this).draggable();
    });
  });
</script>
