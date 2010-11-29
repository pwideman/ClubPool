<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Teams.ViewModels.DetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<p><input type="text" id="name" name="name" class="team-name" value="<%= Html.Encode(Model.Name) %>"/></p>
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
<script type="text/javascript">
  $(document).ready(function () {
    $("#season-results-table tbody:last").removeClass("meet").find("tr:last").addClass("last");
  });
</script>
</asp:Content>
