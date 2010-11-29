<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Teams.ViewModels.DetailsViewModel>" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.UpdateName(null), FormMethod.Post, new { id = "update_name_form" })) { %>
<%= Html.AntiForgeryToken()%>
<%= Html.HiddenFor(m => m.Id) %>
<p><input type="text" id="name" name="name" class="team-name required" value="<%= Html.Encode(Model.Name) %>" title="Click to edit team name"/></p>
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
<%= Html.ScriptInclude("jquery.form.js") %>

<script type="text/javascript">
  var currentTeamName = "";
  $(document).ready(function () {
    // style table
    $("#season-results-table tbody:last").removeClass("meet").find("tr:last").addClass("last");

    // set up team name text box event handlers
    $("#name").blur(saveName).keypress(function (e) {
      if (e.charCode == 13) {
        saveName();
      }
    }).focus(function (e) {
      currentTeamName = $(this).val();
    });
    
    // create ajax form
    $("#update_name_form").ajaxForm({
      success: function (response, status, xhr, form) {
        if (xhr.status === 200) {
          if (response.Success) {
            // update was successful, do nothing
          }
          else {
            // some type of error, probably validation
            // TODO: display error
          }
        }
      },
      error: function (xhr, status, error) {
        // TODO: display server error?
      },
      beforeSubmit: function () {
        return $("#update_name_form").validate().form();
      }
    });

    // set up validation
    $("#update_name_form").validate({
      errorPlacement: function () { }
    });
  });

  function saveName() {
    var newTeamName = $("#name").val();
    if (newTeamName !== currentTeamName) {
      $("#update_name_form").ajaxSubmit();
    }
  }
</script>
</asp:Content>
