﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Seasons.ViewModels.CreateSeasonViewModel>" %>
<div class="form-row">
  <span class="form-label-left"><%= Html.LabelFor(m => m.Name)%></span>
  <div class="form-input">
    <%= Html.TextBoxFor(m => m.Name)%>
    <%= Html.ValidationMessageFor(m => m.Name)%>
  </div>
</div>