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

//[System.Web.Script.Services.ScriptService]
public class GetTestCases : System.Web.Services.WebService 
{
    public GetTestCases () {}

    [WebMethod]
    public XmlDocument WsGetTestCases(string ApplicationName, string ApplicationAreaName)
    {
        var sbResponse = new StringBuilder();
        var xmlResponse = new XmlDocument();

        if (String.IsNullOrEmpty(ApplicationName))
            ApplicationName = "Admin Console";
        ApplicationName = ApplicationName.Replace(" ", "_");

        if (String.IsNullOrEmpty(ApplicationAreaName))
            ApplicationAreaName = "Clients";
        ApplicationAreaName = ApplicationAreaName.Replace(" ", "_");

        sbResponse.Append("<serviceresponse application='" + ApplicationName + "' applicationarea='" + ApplicationAreaName + "'>");

        sbResponse.Append(" <testareas>");

        var mappedPath = Server.MapPath("~");
        var ScriptsDir = mappedPath + "\\" + ConfigurationManager.AppSettings["RegressionTestScriptsDir"];

        var testCasesDirectory = ScriptsDir + "\\" + ApplicationName;
        testCasesDirectory += "\\" + ApplicationAreaName;

        var directories = Directory.GetDirectories(testCasesDirectory);

        if (directories.Length > 0)
        {
            foreach (var directory in directories)
            {
                StringBuilder sbTestFiles = new StringBuilder();

                var tmpDir = directory.Split('\\');
                var dirName = tmpDir[tmpDir.Length - 1].Replace("_", " ");

                // Check to see if there are subdirectories
                var subDirectories = Directory.GetDirectories(directory);
                if (subDirectories.Length > 0)
                {
                    foreach (var subDirectory in subDirectories)
                    {
                        sbResponse.Append("     <case testfiles='" + sbTestFiles.ToString() + "'>" + dirName + "</case>");
                    }
                }
                else
                {
                    sbResponse.Append("<testarea name='" + dirName + "'>");

                    var testFiles = Directory.GetFiles(directory, "*.txt");
                    foreach (var currentFile in testFiles)
                    {
                        var tmpFileName = currentFile.Split('\\');
                        var fileName = tmpFileName[tmpFileName.Length - 1];

                        sbResponse.Append("<testcase>");
                        sbResponse.Append(fileName);
                        sbResponse.Append("</testcase>");
                    }

                    sbResponse.Append("</testarea>");

                    //sbResponse.Append("     <case testfiles='" + sbTestFiles.ToString() + "'>" + dirName + "</case>");
                }
            }
        }
        else
        {
            sbResponse.Append("<testarea name='" + ApplicationAreaName + "'>");

            var testFiles = Directory.GetFiles(testCasesDirectory, "*.txt");
            foreach (var currentFile in testFiles)
            {
                var tmpFileName = currentFile.Split('\\');
                var fileName = tmpFileName[tmpFileName.Length - 1];

                sbResponse.Append("<testcase>");
                sbResponse.Append(fileName);
                sbResponse.Append("</testcase>");
            }

            sbResponse.Append("</testarea>");
        }

        sbResponse.Append(" </testareas>");

        sbResponse.Append("</serviceresponse>");

        xmlResponse.LoadXml(sbResponse.ToString());

        return xmlResponse;
    }
    
}
