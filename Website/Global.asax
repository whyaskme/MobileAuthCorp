<%@ Application Language="C#" %>

<%@ Import namespace="MACServices" %>

<%@ Import namespace="MongoDB.Driver" %>
<%@ Import namespace="MongoDB.Driver.Builders" %>

<script runat="server">

    protected void Application_PreSendRequestHeaders()
    {
        var currentHost = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();

        // Set this response header to only allow resource access to the currently executing environment.
        // This will prevent external sites from linking to our resources such as javascript, css...etc.           
        Response.Headers.Remove("Access-Control-Allow-Origin");
        Response.Headers.Set("Access-Control-Allow-Origin", currentHost);

        // Prevents client proxys from caching user data
        Response.Headers.Remove("Cache-Control");
        Response.Headers.Set("Cache-Control", "No-cache"); // No-cache

        // Mitigates XSS exposure
        Response.Headers.Remove("X-XSS-Protection");
        Response.Headers.Set("X-XSS-Protection", "1"); //       

        // The X-Frame-Options HTTP response header can be used to indicate whether or not a browser should be allowed to render a page in a <frame> or <iframe>. 
        // Sites can use this to avoid Clickjacking attacks, by ensuring that their content is not embedded into other sites. 
        Response.Headers.Remove("X-Frame-Options");
        Response.Headers.Set("X-Frame-Options", "SAMEORIGIN"); // currentHost);        

        // Remove these headers so we don't give hackers too much info
        Response.Headers.Remove("Server");
        Response.Headers.Remove("X-AspNet-Version");
        Response.Headers.Remove("X-AspNetMvc-Version");

        // Headers we want to add
        // HTTP Strict Transport Security (HSTS) is an opt-in security enhancement that is specified by a web application through the use of a special response header. 
        // Once a supported browser receives this header that browser will prevent any communications from being sent over HTTP to the specified domain and will instead send all communications over HTTPS. 
        // It also prevents HTTPS click through prompts on browsers. 
        // Notes: Strict-Transport-Security: max-age=31536000; includeSubDomains; preload
        Response.AddHeader("Strict-Transport-Security", "max-age=300");
    }

    private void Application_Init(object sender, EventArgs e)
    {
    }

    private void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        var mUtils = new MACServices.Utils();

        Application["WebConfigSet"] = false;

        Application["MongoDB"] = mUtils.CreateApplicationDBConnectionPool();
        Application["UserClientList"] = mUtils.GetUsersAndClientIds();

        //string tmpVal = "";
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
