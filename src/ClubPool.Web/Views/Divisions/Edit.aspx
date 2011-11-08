﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Divisions.ViewModels.EditDivisionViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Edit division <%= Html.Encode(Model.Name + " (" + Model.SeasonName + ")") %></span>
  </div>
  <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
       Html.RenderPartial("ErrorMessage");
     } %>
  <div class="form-content">
    <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Divisions.DivisionsController>(c => c.Edit(null), FormMethod.Post, new { @class = "normal" })) { %>
    <fieldset>
      <%= Html.AntiForgeryToken()%>
      <%= Html.HiddenFor(m => m.Id) %>
      <%= Html.HiddenFor(m => m.Version) %>
      <% Html.RenderPartial("DivisionEditControl"); %>
      <div class="spacer">&nbsp;</div>
      <div class="form-row-span">
        <input class="submit-button" type="submit" value="Save" />
      </div>
    </fieldset>
    <% } %>
    <%= Html.ClientSideValidation<ClubPool.Web.Controllers.Divisions.ViewModels.EditDivisionViewModel>() %>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Edit Division
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>