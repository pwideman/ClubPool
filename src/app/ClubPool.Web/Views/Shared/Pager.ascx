<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.PagedListViewModelBase>" %>
<span class="pager-info">Showing <%= Model.First %> - <%= Model.Last %> of <%= Model.Total %></span>
<% if (Model.TotalPages > 1) { %>
<span class="pager-links">
    <span class="pager-arrow-links">
    <% if (Model.CurrentPage > 1) { %>
      <a href="?page=1"><<</a>
      <a href="?page=<%= Model.CurrentPage - 1 %>"><</a>
    <% } else { %>
      << <
    <% } %>
    </span>
    <span class="pager-number-links">
    <% if (Model.TotalPages > 2) {
         var numPageLinksToShow = 5;
         var first = Math.Max(Model.CurrentPage - numPageLinksToShow / 2, 1);
         var last = Math.Min(Model.CurrentPage + numPageLinksToShow / 2, Model.TotalPages);
         if (last - first < numPageLinksToShow - 1) {
           if (1 == first) {
             last += numPageLinksToShow - (last - first) - 1;
           }
           else {
             first -= numPageLinksToShow - (last - first) - 1;
           }
           last = Math.Min(last, Model.TotalPages);
           first = Math.Max(first, 1);
         }
         for (int i = first; i < last + 1; i++) {
           if (i != Model.CurrentPage) { %>
            <a href="?page=<%= i.ToString() %>"><%= i.ToString()%></a>
          <% } else { %>
            <span class="current-page"><%= i.ToString()%></span>
          <% }
         }
       } %>
      </span>
      <span class="pager-arrow-links">
      <% if (Model.CurrentPage < Model.TotalPages) { %>
        <a href="?page=<%= Model.CurrentPage + 1 %>">></a>
        <a href="?page=<%= Model.TotalPages %>">>></a>
      <% } else { %>
        > >>
      <% } %>
    </span>
  <% } %>
  </span>