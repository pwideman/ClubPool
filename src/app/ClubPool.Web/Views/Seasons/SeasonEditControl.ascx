<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Core.SeasonDto>" %>
<div class="form-row">
  <span class="form-label-left"><%= Html.LabelFor(m => m.Name)%></span>
  <div class="form-input">
    <%= Html.TextBoxFor(m => m.Name)%>
    <%= Html.ValidationMessageFor(m => m.Name)%>
  </div>
</div>