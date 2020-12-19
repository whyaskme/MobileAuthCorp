using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Xml;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

//[System.Web.Script.Services.ScriptService]
public class GetTestResults : WebService {

    public GetTestResults () {}

    [WebMethod]
    public XmlDocument WsGetTestRunEnvironments() 
    {
        var sbResponse = new StringBuilder();
        var xmlResponse = new XmlDocument();

        sbResponse.Append("<serviceresponse>");

        sbResponse.Append(" <environments>");

        var mappedPath = Server.MapPath("~");
        var ScriptsDir = mappedPath + "\\WebServices";

        var testRunsDirectory = ScriptsDir + "\\RegressionTestResults";

        var di = new DirectoryInfo(testRunsDirectory);
        var testDirectoriesSorted = di.EnumerateDirectories()
                            .OrderBy(d => d.Name)
                            .Select(d => d.Name)
                            .ToList();

        foreach (var tmpEnv in testDirectoriesSorted)
        {
            sbResponse.Append(" <environment>" + tmpEnv + "</environment>");
        }

        sbResponse.Append(" </environments>");

        sbResponse.Append("</serviceresponse>");

        xmlResponse.LoadXml(sbResponse.ToString());

        return xmlResponse;
    }

    [WebMethod]
    public XmlDocument WsGetTestRuns(string targetEnvironment)
    {
        var sbResponse = new StringBuilder();
        var xmlResponse = new XmlDocument();

        if (string.IsNullOrEmpty(targetEnvironment))
            targetEnvironment = "qa.mobileauthcorp.net";

        var runsToKeep = 20;
        var runCount = 0;

        sbResponse.Append("<serviceresponse>");

        sbResponse.Append(" <testruns runcount=''>");

        var mappedPath = Server.MapPath("~");
        var ScriptsDir = mappedPath + "\\WebServices";

        var testRunsDirectory = ScriptsDir + "\\RegressionTestResults\\" + targetEnvironment;

        var di = new DirectoryInfo(testRunsDirectory);
        var testDirectoriesSorted = di.EnumerateDirectories()
                            .OrderByDescending(d => d.CreationTime)
                            .Select(d => d.Name)
                            .ToList();

        if (testDirectoriesSorted.Count > 0)
        {
            foreach (var resultFolder in testDirectoriesSorted)
            {
                runCount++;

                if (runCount <= runsToKeep)
                {
                    // Return this directory
                    sbResponse.Append(" <testrun runnumber='" + runCount + "'>" + resultFolder + "</testrun>");
                }
                else
                {
                    // Delete this directory to keep the results folder pruned
                    var folderToDelete = testRunsDirectory + "\\" + resultFolder;
                    DirectoryInfo folderToDeleteInfo = new DirectoryInfo(folderToDelete);

                    // Delete the files in the current directory
                    foreach (FileInfo file in folderToDeleteInfo.GetFiles())
                    {
                        file.Delete();
                    }
                    // Delete the current containing folder
                    foreach (DirectoryInfo dir in folderToDeleteInfo.GetDirectories())
                    {
                        dir.Delete(true);
                    }

                    // Finally delete the current root directory
                    folderToDeleteInfo.Delete();
                }
            }
        }

        sbResponse.Append(" </testruns>");

        sbResponse.Replace("runcount=''", "runcount='" + runCount + "'");

        sbResponse.Append("</serviceresponse>");

        xmlResponse.LoadXml(sbResponse.ToString());

        return xmlResponse;
    }

    [WebMethod]
    public XmlDocument WsGetTestRunDetails(string RunToFetch)
    {
        var sbResponse = new StringBuilder();
        var xmlResponse = new XmlDocument();

        sbResponse.Append("<serviceresponse>");

        var tmpVal = RunToFetch.Split(')');
        var tmpVal2 = tmpVal[0].Split('\\');
        var appName = tmpVal2[1].Replace("(", "");

        var appArea = "appArea 1";
        var testArea = "testArea 2";

        var imageFileCount = 0;
        var reportFilePath = "";

        sbResponse.Append(" <testrun appname='" + appName + "' path='" + RunToFetch + "'>");

        var mappedPath = Server.MapPath("~");
        var ScriptsDir = mappedPath + "\\WebServices";

        var testRunsDirectory = ScriptsDir + "\\RegressionTestResults\\" + RunToFetch;

        var testDirectory = new DirectoryInfo(testRunsDirectory);
        var testDirectoriesSorted = testDirectory.EnumerateDirectories()
                            .OrderBy(d => d.Name)
                            .Select(d => d.Name)
                            .ToList();

        if (testDirectoriesSorted.Count > 0)
        {
            sbResponse.Append(" <appareas>");
            foreach (var resultFolder in testDirectoriesSorted)
            {
                sbResponse.Append(" <apparea name='" + resultFolder + "'>");

                sbResponse.Append("     <testareas>");

                // Get subdirectories
                var subDirectoryPath = testRunsDirectory + "\\" + resultFolder;
                var subDirectory = new DirectoryInfo(subDirectoryPath);
                var testSubDirectoriesSorted = subDirectory.EnumerateDirectories()
                                    .OrderBy(d => d.Name)
                                    .Select(d => d.Name)
                                    .ToList();

                foreach (var testSubDirectory in testSubDirectoriesSorted)
                {
                    appArea = resultFolder;
                    testArea = testSubDirectory;

                    sbResponse.Append("<testarea appname='" + appName + "' apparea='" + appArea + "' testarea='" + testArea + "'>");

                    var testAreaPath = testRunsDirectory + "\\" + resultFolder + "\\" + testSubDirectory;
                    var testAreaFolders = new DirectoryInfo(testAreaPath);
                    var testAreaFoldersSorted = testAreaFolders.EnumerateDirectories()
                                        .OrderBy(d => d.Name)
                                        .Select(d => d.Name)
                                        .ToList();

                    foreach(var currentTestAreaFolder in testAreaFoldersSorted)
                    {
                        imageFileCount = 0;
                        reportFilePath = "/"; 

                        var currentFolderName = currentTestAreaFolder;

                        var testResult = "failed";
                        var failedTestCount = 0;

                        sbResponse.Append(" <testfolder name='" + currentFolderName + "' results='results'>");

                        // Get files
                        sbResponse.Append(" <files>");

                        // Get current directory files
                        var filePath = subDirectoryPath + "\\" + testSubDirectory + "\\" + currentTestAreaFolder;

                        string[] filePaths = Directory.GetFiles(filePath);
                        foreach (var currentFile in filePaths)
                        {
                            var fileName = "";
                            var fileExtension = "";

                            reportFilePath = "";

                            // Process path info for file links
                            var pathInfoInitialized = false;
                            var pathInfo = currentFile.Split('\\');
                            for (var i = 0; i < pathInfo.Length; i++)
                            {
                                var currentPathSegment = pathInfo[i];
                                if (currentPathSegment.ToLower() == "webservices")
                                    pathInfoInitialized = true;

                                if(pathInfoInitialized)
                                    reportFilePath += currentPathSegment + "/"; 

                                // This should be the file name
                                if (i == (pathInfo.Length - 1))
                                    fileName = currentPathSegment;
                            }

                            // Trim trailing slash off url
                            reportFilePath = reportFilePath.Substring(0, reportFilePath.Length - 1);

                            StringBuilder sbFile = new StringBuilder();

                            var tmpFileInfo = fileName.Split('.');
                            fileExtension = tmpFileInfo[1];
                            switch (fileExtension)
                            {
                                case "bat": // Batch file that ran this test - Do nothing!
                                    break;

                                case "html": // Log and Report html files
                                    sbResponse.Append("<file type='" + fileExtension + "' path='" + reportFilePath + "'>");
                                    sbResponse.Append(fileName.Replace(@"\", @"/").Replace(@".html", @""));
                                    sbResponse.Append("</file>");
                                    break;

                                case "png": // Screen cap image files
                                    imageFileCount++;
                                    sbResponse.Append("<file type='" + fileExtension + "' path='" + reportFilePath + "'>");
                                    sbResponse.Append("Img-" + imageFileCount);
                                    sbResponse.Append("</file>");
                                    break;

                                case "xml":
                                    // If output.xml, read the result elements
                                    if (!string.IsNullOrEmpty(currentFile))
                                    {
                                        XmlDocument resultsDoc = new XmlDocument();
                                        resultsDoc.Load(currentFile);

                                        XmlNodeList statTotal = resultsDoc.GetElementsByTagName("total");
                                        foreach (XmlNode currentElement in statTotal)
                                        {
                                            for (var i = 0; i < currentElement.ChildNodes.Count; i++)
                                            {
                                                failedTestCount = Convert.ToInt16(currentElement.ChildNodes[i].Attributes["fail"].Value);
                                            }
                                        }
                                        if (failedTestCount < 1)
                                            testResult = "passed";
                                    }

                                    // Update the results
                                    sbResponse = sbResponse.Replace("results='results'", "results='" + testResult + "'");

                                    sbResponse.Append("<file type='" + fileExtension + "' path='" + reportFilePath + "'>");
                                    sbResponse.Append("Xml");
                                    sbResponse.Append("</file>");
                                    break;

                                case "txt": // Execution results
                                    sbResponse.Append("<file type='" + fileExtension + "' path='" + reportFilePath + "'>");
                                    sbResponse.Append("Txt");
                                    sbResponse.Append("</file>");
                                    break;
                            }
                        }
                        sbResponse.Append(" </files>");

                        sbResponse.Append(" </testfolder>");
                    }

                    sbResponse.Append("</testarea>");
                }

                sbResponse.Append("     </testareas>");

                sbResponse.Append("</apparea>");
            }
            sbResponse.Append(" </appareas>");
        }

        sbResponse.Append(" </testrun>");

        sbResponse.Append("</serviceresponse>");

        xmlResponse.LoadXml(sbResponse.ToString());

        return xmlResponse;
    }
}
