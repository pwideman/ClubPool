<%@ Page Language="C#" %>

<%-- DO NOT DELETE THIS FILE. It is used to ensure that ASP.NET MVC is activated by IIS when a user makes a "/" request to the server. --%>

<script runat="server">
  public void Page_Load(object sender, System.EventArgs e) {
    HttpContext.Current.RewritePath(Request.ApplicationPath, false);
    IHttpHandler httpHandler = new MvcHttpHandler();
    httpHandler.ProcessRequest(HttpContext.Current);
  }
</script>

