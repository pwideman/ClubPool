<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Home.ViewModels.AboutViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h5>About <%= Model.SiteName %></h5>

<h5>About this website</h5>
<p>This website is best viewed in anything other than IE. The site will work in
IE8 but does not look as nice and will mostly work in IE7, but works best in
Firefox, Chrome, Safari, etc. IE6 and below will not work at all.</p>
<p>This website is based on the ClubPool project located at <a href="http://github.com/pwideman/clubpool/">
http://github.com/pwideman/clubpool/</a>. ClubPool is built with the following components:</p>
<ul>
  <li><a href="http://www.asp.net/mvc">ASP.NET MVC</a></li>
  <li><a href="http://mvccontrib.codeplex.com/">MvcContrib</a></li>
  <li><a href="http://sharparchitecture.net">Sharp Architecture</a></li>
  <li><a href="http://nhforge.org">NHibernate</a></li>
  <li><a href="http://fluentnhibernate.org/">Fluent NHibernate</a></li>
  <li><a href="http://nhforge.org/wikis/validator/default.aspx">NHibernate Validator</a></li>
  <li><a href="http://www.dotlesscss.org/">.Less Css</a></li>
  <li><a href="http://castleproject.org/">Castle Windsor</a></li>
  <li><a href="http://code.google.com/p/elmah/">Elmah</a></li>
  <li><a href="http://xval.codeplex.com/">xVal</a></li>
  <li><a href="http://jquery.com/">jQuery</a></li>
  <li><a href="http://jqueryui.com/">jQuery UI</a></li>
  <li>jQuery plugins:
    <ul>
      <li><a href="http://www.itgroup.com.ph/alphanumeric/">jQuery AlphaNumeric</a></li>
      <li><a href="http://docs.jquery.com/Plugins/bgiframe">jquery.bgiframe</a></li>
      <li><a href="http://github.com/carhartl/jquery-cookie">jquery.cookie</a></li>
      <li><a href="http://jquery.malsup.com/corner/">jquery.corner</a></li>
      <li><a href="http://jupiterjs.com/news/delegate-able-hover-events-for-jquery">jquery.event.hover</a></li>
      <li><a href="http://malsup.com/jquery/form/">jquery.form</a></li>
      <li><a href="http://cherne.net/brian/resources/jquery.hoverIntent.html">jquery.hoverIntent</a></li>
      <li><a href="http://keith-wood.name/timeEntry.html">jquery.timeentry</a></li>
      <li><a href="http://bassistance.de/jquery-plugins/jquery-plugin-validation/">jQuery Validation</a></li>
      <li><a href="http://users.tpg.com.au/j_birch/plugins/superfish">jQuery Superfish menus (with supersubs)</a></li>
    </ul>
  </li>
</ul>

<p>Special thanks to the team behind the <a href="http://whocanhelpme.codeplex.com/">Who Can Help Me?</a> sample application for all of the great ideas.</p>

<p>Icons used:
  <ul>
    <li><a href="http://dryicons.com">Coquette by DryIcons</a></li>
    <li><a href="http://kyo-tux.deviantart.com/">Phuzion by kyo-tux</a></li>
  </ul>
</p>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
About <%= Model.SiteName %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
