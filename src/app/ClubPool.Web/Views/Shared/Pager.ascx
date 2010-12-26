<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.PagedListViewModelBase>" %>
<span class="pager-info">Showing <%= Model.First %> - <%= Model.Last %> of <%= Model.Total %></span>
<% if (Model.TotalPages > 1) { %>
<ul class="pager">
  <% if (Model.CurrentPage > 1) { %>
    <li class="first"><a href="?page=1">« First</a></li>
    <li class="previous"><a href="?page=<%= Model.CurrentPage - 1 %>">« Prev</a></li>
  <% } else { %>
    <li class="first disabled">« First</li>
    <li class="previous disabled">« Prev</li>
  <% } %>
  <% if (Model.TotalPages > 2) {
       for (int i = Model.FirstPageNumberLink; i < Model.LastPageNumberLink + 1; i++) {
         if (i != Model.CurrentPage) { %>
         <li class="page"><a href="?page=<%= i.ToString() %>"><%= i.ToString()%></a></li>
       <% } else { %>
         <li class="currentpage"><%= i.ToString()%></li>
       <% }
       }
     } %>
    <% if (Model.CurrentPage < Model.TotalPages) { %>
      <li class="next"><a href="?page=<%= Model.CurrentPage + 1 %>">Next »</a></li>
      <li class="last"><a href="?page=<%= Model.TotalPages %>">Last »</a></li>
    <% } else { %>
      <li class="next disabled">Next »</li>
      <li class="last disabled">Last »</li>
    <% } %>
<% } %>
</ul>