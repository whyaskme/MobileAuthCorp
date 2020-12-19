using System;
using System.Web;
using System.Web.Services;

using MACServices;

/// <summary>
/// Summary description for EventLog
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class EventLog : WebService {

    // ReSharper disable once EmptyConstructor
    public EventLog () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string wsEventLog()
    {
        Event oldEvent = new Event();
        oldEvent.EventTypeName = "Old Event IP";
        oldEvent.EventTypeDesc = "This IP is incorrect (Internally set inside class)";
        oldEvent.Create();

        // ReSharper disable once UnusedVariable
        var UserIpAddress = String.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"])
        ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
        : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //EventTest newEvent = new EventTest();
        //newEvent.EventTypeName = "New Event IP";
        //newEvent.EventTypeDesc = "This IP is correct (Externally set outside class)";
        //newEvent.UserIpAddress = UserIpAddress;
        //newEvent.Create();

        return "Event Log";
    }
    
}
