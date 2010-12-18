<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="ui-state-error ui-corner-all message"> 
	<p><span class="ui-icon ui-icon-alert message-icon"></span> 
	<%= Html.Encode(TempData[GlobalViewDataProperty.PageErrorMessage]) %></p>
</div>

