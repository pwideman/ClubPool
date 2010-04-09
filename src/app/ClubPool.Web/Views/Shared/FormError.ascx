<%@ Control Language="C#" Inherits="ClubPool.Web.Views.AspxViewUserControlBase<ClubPool.Web.Controllers.FormViewModelBase>" %>
<div class="ui-state-error ui-corner-all form-error">
  <p>
    <span class="ui-icon ui-icon-alert form-error-icon"></span>
    <%= Model.ErrorMessage %>
  </p>
</div>
