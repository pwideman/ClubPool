<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Matches.ViewModels.UserHistoryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h4>Match History for <%= Html.Encode(Model.Name) %></h4>
<% if (Model.HasMatches) { %>
<div>
  <table class="domain-list match-history">
    <thead>
      <tr>
        <th>Season</th>
        <th>Team 1</th>
        <th>Player 1</th>
        <th>I</th>
        <th>D</th>
        <th>W</th>
        <th>Team 2</th>
        <th>Player 2</th>
        <th>I</th>
        <th>D</th>
        <th>W</th>
        <th>Winner</th>
      </tr>
    </thead>
    <tbody class="content">
  <% foreach (var item in Model.Items) { %>
      <tr>
        <td><%= Html.Encode(item.Season)%></td>
        <td><%= Html.Encode(item.Team1) %></td>
        <td><%= Html.Encode(item.Player1)%></td>
        <td><%= Html.Encode(item.Player1Innings)%></td>
        <td><%= Html.Encode(item.Player1DefensiveShots)%></td>
        <td><%= Html.Encode(item.Player1Wins)%></td>
        <td><%= Html.Encode(item.Team2) %></td>
        <td><%= Html.Encode(item.Player2)%></td>
        <td><%= Html.Encode(item.Player2Innings)%></td>
        <td><%= Html.Encode(item.Player2DefensiveShots)%></td>
        <td><%= Html.Encode(item.Player2Wins)%></td>
        <td><%= Html.Encode(item.Winner) %></td>
      </tr>
  <% } %>
    </tbody>
    <tr class="pager">
      <td colspan="99">
        <% Html.RenderPartial("Pager"); %>
      </td>
    </tr>
  </table>
</div>
<% } else { %>
<p>This user has no match history.</p>
<% } %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
User History
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<script type="text/javascript">
  $(function () {
    $("tbody.content tr:odd").addClass("alt");
  });
</script>
</asp:Content>
