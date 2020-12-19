using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;

using MACServices;
using cs = MACServices.Constants.Strings;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[ScriptService]
public class AdPassServices : System.Web.Services.WebService {

    public AdPassServices () {}

    [WebMethod]
    public XmlDocument WsGetAdPassClientInfoByName(string clientName)
    {

        var sbResponse = new StringBuilder();
        var xmlResponse = new XmlDocument();

        //var adPassServiceUrl = "api.authenticationads.com/Ad.svc";

        if (string.IsNullOrEmpty(clientName))
            clientName = "!MAC Default Client";

        sbResponse.Append("<serviceresponse clientname='" + clientName + "'>");

        try
        {
            sbResponse.Append("<clientinfo>");
            sbResponse.Append("<clientid>" + ObjectId.GenerateNewId() + "</clientid>");
            sbResponse.Append("<apikey>" + ObjectId.GenerateNewId() + "</apikey>");
            sbResponse.Append("<username>Temp UserName</username>");
            sbResponse.Append("<password>Temp Password</password>");
            sbResponse.Append("</clientinfo>");
        }
        catch(Exception ex)
        {
            sbResponse.Append(ex.ToString());
        }

        sbResponse.Append("</serviceresponse>");

        xmlResponse.LoadXml(sbResponse.ToString());

        return xmlResponse;
    }
    
}
