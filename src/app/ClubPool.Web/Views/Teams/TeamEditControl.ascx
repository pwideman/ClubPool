<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Teams.ViewModels.TeamViewModel>" %>

<span id="PlayerIdList">
<% foreach(var player in Model.Players) { %>
<input type="hidden" name="Players" id="PlayerId<%= player.Id %>" value="<%= player.Id %>" />
<% } %>
</span>
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
    <% Html.RenderPartial("PlayerListView", Model.Players); %>
  </span>
</div>
<div class="form-row" style="margin: 10px;padding:0; min-height:0">
  <span class="form-label-left">&nbsp;</span>
  <div class="form-input">Drag users to/from Players and Available Players</div>
</div>
<div class="form-row">
  <span class="form-label-left"><%= Html.LabelFor(m => m.AvailablePlayers) %></span>
  <span id="AvailablePlayersList">
    <% Html.RenderPartial("PlayerListView", Model.AvailablePlayers); %>
  </span>
</div>
<script type="text/javascript">
  var playersList, availablePlayersList;

  function updatePlayersListState() {
    var accept = "";
    if (playersList.children().length >= 2) {
      accept = ".accept-none";
    } else {
      accept = ".available-player";
    }
    playersList.parent().droppable("option", "accept", accept);
  }

  $(document).ready(function () {
    playersList = $("#PlayersList > div > ul");
    availablePlayersList = $("#AvailablePlayersList > div > ul");

    playersList.sortable().parent().droppable({
      accept: ".available-player",
      drop: function (event, ui) {
        $(this).removeClass("simple-user-list-over").data("test");
        playersList.append(ui.draggable);
        ui.draggable.css("top", "0").css("left", "0").removeClass("available-player").addClass("player");
        updatePlayersListState();
        var playerId = ui.draggable.attr("id");
        $("#PlayerIdList").append("<input type='hidden' name='Players' id='PlayerId" + playerId + "' value='" + playerId + "'/>");
      },
      over: function (event, ui) {
        $(this).addClass("simple-user-list-over");
      },
      out: function (event, ui) {
        $(this).removeClass("simple-user-list-over");
      }
    });

    availablePlayersList.sortable().parent().droppable({
      accept: ".player",
      drop: function (event, ui) {
        $(this).removeClass("simple-user-list-over");
        availablePlayersList.append(ui.draggable);
        ui.draggable.css("top", "0").css("left", "0").removeClass("player").addClass("available-player");
        updatePlayersListState();
        var playerId = ui.draggable.attr("id");
        $("#PlayerId" + playerId).remove();
      },
      over: function (event, ui) {
        $(this).addClass("simple-user-list-over");
      },
      out: function (event, ui) {
        $(this).removeClass("simple-user-list-over");
      }
    });

    playersList.children().each(function () {
      $(this).addClass("player").draggable({
        revert: "invalid"
      });
    });

    availablePlayersList.children().each(function () {
      $(this).addClass("available-player").draggable({
        revert: "invalid"
      });
    });

    updatePlayersListState();
  });
</script>
