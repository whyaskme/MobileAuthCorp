using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

// [System.Web.Script.Services.ScriptService]
public class GetApplicationAreas : System.Web.Services.WebService {

    public GetApplicationAreas () {}

    [WebMethod]
    public XmlDocument WsGetApplicationAreas(string ApplicationName)
    {
        if (String.IsNullOrEmpty(ApplicationName))
            ApplicationName = "Admin Console";
        ApplicationName = ApplicationName.Replace(" ", "_");

        var sbResponse = new StringBuilder();
        var xmlResponse = new XmlDocument();

        var mappedPath = Server.MapPath("~");
        var ScriptsDir = mappedPath + "\\" + ConfigurationManager.AppSettings["RegressionTestScriptsDir"];

        var testCasesDirectory = ScriptsDir + "\\" + ApplicationName;

        sbResponse.Append("<serviceresponse application='" + ApplicationName + "'>");

        sbResponse.Append(" <applicationareas>");

        var directories = Directory.GetDirectories(testCasesDirectory);
        foreach (var di in directories)
        {
            var tmpDir = di.Split('\\');
            var dirName = tmpDir[tmpDir.Length - 1].Replace("_", " ");

            sbResponse.Append("     <area>" + dirName + "</area>");
        }
        sbResponse.Append(" </applicationareas>");

        sbResponse.Append("</serviceresponse>");

        xmlResponse.LoadXml(sbResponse.ToString());

        return xmlResponse;
    }
}
