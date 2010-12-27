<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Contact.ViewModels.ContactViewModelBase>" %>

<div class="contact-view">
  <div class="contact-view-row">
    <div class="contact-view-label"><%= Html.LabelFor(m => m.ReplyToAddress) %></div>
    <div class="contact-view-field"><%= Html.TextBoxFor(m => m.ReplyToAddress) %></div>
  </div>
  <div class="contact-view-row">
    <div class="contact-view-label"><%= Html.LabelFor(m => m.Subject) %></div>
    <div class="contact-view-field"><%= Html.TextBoxFor(m => m.Subject) %></div>
  </div>
  <div class="contact-view-row">
    <div class="contact-view-label"><%= Html.LabelFor(m => m.Body) %></div>
    <div class="contact-view-field"><%= Html.TextAreaFor(m => m.Body) %></div>
  </div>
  <div class="contact-view-row">
    <div class="contact-view-label">&nbsp;</div>
    <div class="contact-view-field"><%= Html.SubmitButton("Send") %></div>
  </div>
</div>