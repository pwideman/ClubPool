<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Meets.ViewModels.MeetViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Match Details</span>
  </div>
  <p>
    <strong><%= Model.Team1.Name %></strong> vs. <strong><%= Model.Team2.Name %></strong>, 
    scheduled for week <%= Model.ScheduledWeek %> (<%= Model.ScheduledDate.ToShortDateString() %>)
  </p>
  <table>
    <thead>
      <tr>
        <th>Player 1</th>
        <th>Player 2</th>
        <th>Result</th>
      </tr>
    </thead>
    <tbody>
      <% foreach(var match in Model.Matches) { %>
      <tr>
        <td><%= match.Player1.Name %></td>
        <td><%= match.Player2.Name %></td>
        <td>
        <% if (match.IsComplete) { %>
          Winner: <%= match.Winner.Name%>
        <% } else { %>
          Incomplete
        <% } %>
        </td>
      </tr>
      <% } %>
    </tbody>
  </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Match Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
