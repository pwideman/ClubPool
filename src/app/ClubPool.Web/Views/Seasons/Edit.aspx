<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Core.SeasonDto>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Edit Season</span>
  </div>
  <div class="form-content edit-season-properties-form">
    <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
          Html.RenderPartial("ErrorMessage");
        } 
        using (var form = Html.BeginForm<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.Edit(null), FormMethod.Post, new { @class = "normal" })) {
    %>
    <fieldset>
      <%= Html.AntiForgeryToken()%>
      <% Html.RenderPartial("SeasonEditControl"); %>
      <div class="spacer">&nbsp;</div>
      <div class="form-row-span">
        <input class="submit-button" type="submit" value="Save" />
      </div>
    </fieldset>
    <% } %>
    <%= Html.ClientSideValidation<ClubPool.Core.SeasonDto>() %>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
