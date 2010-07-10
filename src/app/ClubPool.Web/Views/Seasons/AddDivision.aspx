<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Seasons.ViewModels.AddDivisionViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Add Division to <%= Model.SeasonName %></span>
  </div>
  <div class="form-content">
    <div class="form-header">
      All fields are required
    </div>
    <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
          Html.RenderPartial("ErrorMessage");
        }
       using (var form = Html.BeginForm<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.AddDivision(null), FormMethod.Post, new { @class = "normal" })) {
    %>
    <fieldset>
      <%= Html.AntiForgeryToken()%>
      <%= Html.HiddenFor(m => m.SeasonId)%>
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
      <div class="spacer">&nbsp;</div>
      <div class="form-row-span">
        <input class="submit-button" type="submit" value="Create Division" />
      </div>
    </fieldset>
    <% } %>
    <%= Html.ClientSideValidation<ClubPool.Web.Controllers.Seasons.ViewModels.AddDivisionViewModel>() %>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Add Division - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
