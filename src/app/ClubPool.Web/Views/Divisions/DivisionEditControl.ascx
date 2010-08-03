﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Divisions.ViewModels.DivisionViewModel>" %>
<div class="form-row">
  <span class="form-label-left"><%= Html.LabelFor(m => m.Name)%></span>
  <div class="form-input">
    <%= Html.TextBoxFor(m => m.Name)%>
    <%= Html.ValidationMessageFor(m => m.Name)%>
  </div>
</div>
<div class="form-row">
  <span class="form-label-left"><%= Html.LabelFor(m => m.StartingDate)%></span>
  <div class="form-input">
    <%= Html.TextBoxFor(m => m.StartingDate)%>
    <%= Html.ValidationMessageFor(m => m.StartingDate)%>
  </div>
</div>
