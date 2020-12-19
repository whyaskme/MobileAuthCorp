<%@ Application Language="C#" %>

<script runat="server">

    protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
    {
        //var tmpVal = "Application_PreSendRequestHeaders";
    }

    void Application_PreSendRequestContent(object sender, EventArgs e)
    {
        // This is called last before XStreamingElement response ToolboxDataAttribute callee
        if (Response.ContentType == "text/xml; charset=utf-8")
        {
            // Set the content type correcly
            Response.ContentType = "application/json; charset=utf-8";          
        }
    }   

    void Application_PostReleaseRequestState(object sender, EventArgs e)
    {
        // Here is where we need to strip the xml text element wrapper from the response
        if (Response.ContentType == "text/xml; charset=utf-8" || Response.ContentType == "application/json")
        {
            Response.Filter = new ModifyResponseStream(Response.Filter);
        }
    }     

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
        if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
            HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
            HttpContext.Current.Response.End();
        }
    }

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }
       
</script>
