<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.FormViewModelBase>" %>
<div class="ui-state-error ui-corner-all form-error">
  <p>
    <span class="ui-icon ui-icon-alert form-error-icon"></span>
    <%= Html.Encode(TempData[GlobalViewDataProperty.PageErrorMessage]) %>
  </p>
</div>
