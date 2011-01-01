﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.PagedListViewModelBase>" %>
<%-- Use of this control requires the jquery.query plugin --%>

<span class="pager-info">Showing <%= Model.First %> - <%= Model.Last %> of <%= Model.Total %></span>
<% if (Model.TotalPages > 1) { %>
<ul class="pager">
  <% if (Model.CurrentPage > 1) { %>
    <li class="pagelink first" data-page="1">« First</li><li class="pagelink previous" data-page="<%= Model.CurrentPage - 1%>">« Prev</li>
  <% } else { %>
    <li class="first disabled">« First</li><li class="previous disabled">« Prev</li>
  <% } %>
  <% if (Model.TotalPages > 2) {
       for (int i = Model.FirstPageNumberLink; i < Model.LastPageNumberLink + 1; i++) {
         if (i != Model.CurrentPage) { %>
           <li class="pagelink page" data-page="<%= i.ToString() %>"><%= i.ToString()%></li>
      <% } else { %>
           <li class="currentpage"><%= i.ToString()%></li>
      <% }
       }
     } %>
    <% if (Model.CurrentPage < Model.TotalPages) { %>
      <li class="pagelink next" data-page="<%= Model.CurrentPage + 1 %>">Next »</li><li class="pagelink last" data-page="<%= Model.TotalPages %>">Last »</li>
    <% } else { %>
      <li class="next disabled">Next »</li><li class="last disabled">Last »</li>
    <% } %>
<% } %>
</ul>

<script type="text/javascript">
  $(function () {
    $(".pagelink").click(function () {
      location.assign($.query.set("page", $(this).data("page")));
    });
  });
</script>