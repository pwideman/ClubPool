<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.CurrentSeason.ViewModels.StandingsPlayerViewModel>" %>

<% if (null != Model) { %>
<td><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.View(Model.Id), Model.Name)%></td>
<td><%= Model.SkillLevel%></td>
<td><%= string.Format("{0} - {1}", Model.Wins, Model.Losses)%></td>
<td><%= string.Format("{0:0.00}", Model.WinPercentage)%></td>
<% } else { %>
<td/>
<td/>
<td/>
<td/>
<% } %>
