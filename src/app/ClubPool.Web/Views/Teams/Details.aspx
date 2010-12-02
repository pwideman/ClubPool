<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Teams.ViewModels.DetailsViewModel>" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<% if (Model.CanUpdateName) { %>
<% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.UpdateName(null), FormMethod.Post, new { id = "update_name_form" })) { %>
<%= Html.AntiForgeryToken()%>
<%= Html.HiddenFor(m => m.Id) %>
<p><input type="text" id="name" name="name" class="team-name required" size="30" value="<%= Html.Encode(Model.Name) %>" title="Click to edit team name, enter or tab out to save"/></p>
<% }
   }
   else { %>
<h4><%= Model.Name%></h4>
<% } %>
<div class="container">
  <div class="header">Details</div>
  <div class="content">
    <ul id="team-details">
      <li>
        Players:
        <ul>
          <% foreach (var player in Model.Players) { %>
          <li><%= Html.Encode(string.Format("{0} ({1})", player.Name, player.EightBallSkillLevel)) %></li>
          <% } %>
        </ul>
      </li>
      <li>Record: <%= Html.Encode(Model.Record) %></li>
      <li>Division Ranking: <%= Html.Encode(Model.Rank) %></li>
    </ul>
  </div>
</div>
<br/>
<% if (Model.HasSeasonResults) { %>
<div class="container">
  <div class="header">Season Results</div>
  <div class="content">
    <table id="season-results-table">
      <thead>
        <tr>
          <th>Opponent</th>
          <th>Player</th>
          <th>Wins</th>
          <th>Player</th>
          <th>Wins</th>
          <th>Result</th>
        </tr>
      </thead>
        <% foreach (var meet in Model.SeasonResults) { %>
        <tbody class="meet">        
        <%   foreach (var match in meet.Matches) { %>
          <tr <%= match.Win ? @"class=""winner""" : "" %>>
            <td><%= Html.Encode(meet.Opponent)%></td>
            <td><%= Html.Encode(match.OpponentPlayerName)%></td>
            <td><%= Html.Encode(match.OpponentPlayerWins)%></td>
            <td><%= Html.Encode(match.TeamPlayerName)%></td>
            <td><%= Html.Encode(match.TeamPlayerWins)%></td>
            <td><%= Html.Encode(match.Win ? "W" : "L")%></td>
          </tr>
        <%   } %>
        </tbody>
        <% } %>
      </tbody>
    </table>
  </div>
</div>
<% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Team Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<% if (Model.CanUpdateName) { %>
<%= Html.ScriptInclude("jquery.form.js") %>

<script type="text/javascript">
  var currentTeamName = "<%= Model.Name %>";
  // declare formOpts explicitly, since we need to use it in two different places
  var formOpts = {
    success: function (response, status, xhr, form) {
      if (xhr.status === 200) {
        if (response.Success) {
          currentTeamName = $("#name").val();
          $("#name").effect("highlight", 1500);
        }
        else {
          $("#name").ajaxUpdateError({ message: response.Message });
          $("#name").val(currentTeamName);
        }
      }
    },
    error: function (xhr, status, error) {
      $("#name").ajaxUpdateError({ message: "An error occurred on the server while saving your changes, try again" });
    },
    beforeSubmit: function () {
      $log("beforeSubmit");
      var valid = $("#update_name_form").validate().form();
      $log("valid: " + valid);
      var different = $("#name").val() != currentTeamName;
      $log("different: " + different);
      var ret = valid && different;
      $log("returning: " + ret);
      $("#name").ajaxUpdateError("close");
      return ret;
    }
  };

  $(function () {
    // set up team name text box event handlers
    $("#name").blur(saveName).focus(function (e) {
      currentTeamName = $(this).val();
    });
    // create ajax form
    $("#update_name_form").ajaxForm(formOpts);

    // set up validation
    $("#update_name_form").validate({
      errorPlacement: function () { }
    });
  });

  function saveName() {
    $("#update_name_form").ajaxSubmit(formOpts);
  }
</script>
<% } %>

<script type="text/javascript">
  $(function() {
    // style table
    $("#season-results-table tbody:last").removeClass("meet").find("tr:last").addClass("last");
  });
</script>
</asp:Content>
