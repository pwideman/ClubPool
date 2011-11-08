<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="ui-state-highlight ui-corner-all message notification">
	<p><span class="ui-icon ui-icon-info message-icon"></span>
	<%= Html.Encode(TempData[GlobalViewDataProperty.PageNotificationMessage]) %></p>
</div>