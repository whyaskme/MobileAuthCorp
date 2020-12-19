using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Xml;

using MACSecurity;
using MACServices;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using dkui = MACServices.Constants.Dictionary.Userinfo;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[System.Web.Script.Services.ScriptService]
public class EndUserSMSReply : WebService
{

    public Object PropertyValue { get; set; }
    public virtual Type PropertyType { get; set; }

    [WebMethod]
    public XmlDocument WsEndUserSMSReply(string data)
    {
        var mUtils = new Utils();

        //Tuple<string, string> request;
        // request data Dictionary
        var myData = new Dictionary<string, string> {{dk.ServiceName, "EndUserSMSReply"}};

        // start the XML response
        var myResponse = new StringBuilder();
        mUtils.InitializeXmlResponse(myResponse);

        return mUtils.EventLogError_FinalizeXmlResponse(myData[dk.ServiceName],
                "", "Not Implemented!" + Environment.NewLine + data, "99");
    }
}
