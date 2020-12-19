using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Xml;

[WebService(Namespace = "http://mobileauthcorp.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

public class RunTestService : WebService
{
    public const string URL = "URL";
    public const string BROWSER = "BROWSER";
    public const string PATH_KEY = "PATH";
    public const string USER = "USER";
    public const string CLIENT = "CLIENT";
    public const string GROUP = "GROUP";
    public const string ItemSep = "|";
    public const string KVSep = ":";
    public const string OUTPUTFILE = "output.xml";
    public const string RUNID_KEY = "RUNID";
    public bool ResultSummary = false;

    [WebMethod]
    //public string WsRunTestService(string SUT, string pathToTestScript)
    public XmlDocument WsRunTestService(string data)
    {
        var sbResponse = new StringBuilder();
        var xmlResponse = new XmlDocument();
        if (data.Length < 10)
        {
            // create run id (day of year + time of run
            var dt = DateTime.Now;
            var RunId = String.Format("Run{0}{1}", dt.DayOfYear, dt.ToString("hhmmss"));

            // ReSharper disable once RedundantAssignment
            var mTargetURL = String.Empty;
            if (data.StartsWith("l"))
                mTargetURL = "http://localhost";
            else if (data.StartsWith("t"))
                mTargetURL = "http://test-integration.mobileauthcorp.net";
            else if (data.StartsWith("q"))
                mTargetURL = "http://qa.mobileauthcorp.net";
            else
                mTargetURL = "http://localhost";

            var data1 = URL + KVSep + mTargetURL
                + ItemSep + RUNID_KEY + KVSep + RunId
                + ItemSep + USER + KVSep + "QAUser@Mobileauthcorp.com"
                + ItemSep + CLIENT + KVSep + "Coffee Shop"
                + ItemSep + GROUP + KVSep + "Test Group"
                + ItemSep + PATH_KEY + KVSep + @"\TestCases\Admin_Console\Clients\Select_Client\Select_Client.txt";

            if (data.EndsWith("f"))
                data1 += ItemSep + BROWSER + KVSep + "Firefox";
            else if (data.EndsWith("c"))
                data1 += ItemSep + BROWSER + KVSep + "Chrome";
            else if (data.EndsWith("i"))
                data1 += ItemSep + BROWSER + KVSep + "IE";
            else if (data.EndsWith("s"))
                data1 += ItemSep + BROWSER + KVSep + "Safari";

            data = StringToHex(data1);
        }

        var mURL = String.Empty;
        var mUser = String.Empty;
        var mClient = String.Empty;
        var mGroup = String.Empty;
        var mParticalPathToTestScript = String.Empty;
        var mRunId = String.Empty;
        var mBrowser = String.Empty;
        // convert to string
        var mData = HexToString(data);
        // parameters to array
        var mParas = mData.Split(char.Parse(ItemSep));
        // parse each parameter in input data
        foreach (var mPara in mParas)
        {
            if (mPara.StartsWith(URL))
                mURL = mPara.Replace(URL + KVSep, "");
            if (mPara.StartsWith(USER))
                mUser = "--variable \"USER:" + mPara.Replace(USER + KVSep, "") + "\"";
            if (mPara.StartsWith(BROWSER))
                mBrowser = "--variable BROWSER:" + mPara.Replace(BROWSER + KVSep, "");
            if (mPara.StartsWith(CLIENT))
                mClient = "--variable \"CLIENT:" + mPara.Replace(CLIENT + KVSep, "") + "\"";
            if (mPara.StartsWith(GROUP))
                mGroup = "--variable \"GROUP:" + mPara.Replace(GROUP + KVSep, "") + "\"";

            if (mPara.StartsWith(PATH_KEY))
                mParticalPathToTestScript = mPara.Replace(PATH_KEY + KVSep, "");
            if (mPara.StartsWith(RUNID_KEY))
                mRunId = mPara.Replace(RUNID_KEY + KVSep, "");
        }

        //var mRunResultsFolder = mResultsFolder + "/" + mRunId;
        var tmpVal = mParticalPathToTestScript.Split('\\');
        var appName = tmpVal[2];
        var appArea = tmpVal[3];
        var testArea = tmpVal[4];

        var fullOutputPath = appArea + "\\" + testArea.Replace(" ", "_");

        mParticalPathToTestScript = mParticalPathToTestScript.Replace(" ", "_");

        sbResponse.Append("<serviceresponse ");
        sbResponse.Append("systemundertest='" + mURL + "' ");
        sbResponse.Append("testscript='" + HttpUtility.UrlEncode(mParticalPathToTestScript) + "' ");
        sbResponse.Append("runid='" + mRunId + "' ");
        sbResponse.Append("testpassed=testpassed");
        sbResponse.Append(">");

        sbResponse.Append("<testresult>");

        if (string.IsNullOrEmpty(mURL))
            sbResponse.Append("  Error No url for system under test!");

        if (string.IsNullOrEmpty(mParticalPathToTestScript))
            sbResponse.Append("  Error No Path To Test Script!");

        if (mParticalPathToTestScript.EndsWith(".txt") == false)
            sbResponse.Append("  Error Invalid Test Script file type!");

        var mExePath = Server.MapPath("~");
        var mPathToTestScript = mExePath + mParticalPathToTestScript;

        if (File.Exists(mPathToTestScript) == false)
            sbResponse.Append("  Error Test Script file does not exist!");

        var mTestName = Path.GetFileNameWithoutExtension(mPathToTestScript);

        // create results folder using run id
        var mResultsFolder = Server.MapPath("RegressionTestResults");

        var result = CreateDir(mResultsFolder);
        if (!String.IsNullOrEmpty(result))
            sbResponse.Append(result);

        var mComputer = Environment.MachineName.ToUpper();

        var serverName = mURL.Replace("http://", "").ToLower();

        if (serverName.Contains("localhost"))
        {
            switch (mComputer.ToLower())
            {
                case "chrismiller":
                    serverName += "-chris";
                    break;
                case "terryshotbox":
                    serverName += "-terry";
                    break;
                case "lenovo-pc":
                    serverName += "-joe";
                    break;
            }
        }

        var mRunResultsFolder = mResultsFolder + "\\" + serverName + "\\(" + appName + ")_" + mRunId;

        result = CreateDir(mRunResultsFolder);

        if (!String.IsNullOrEmpty(result))
            sbResponse.Append(result);

        var mTestResultsFolder = mRunResultsFolder + "\\" + fullOutputPath + "\\" + mTestName;

        result = CreateDir(mTestResultsFolder);
        if (!String.IsNullOrEmpty(result))
            sbResponse.Append(result);

        // create command to run
        var mCmd = String.Format("pybot --variable URL:{0} {1} {2} {3} {4} --outputdir \"{5}\" \"{6}\"{7}",
                mURL, mBrowser, mUser, mClient, mGroup,
                mTestResultsFolder, mPathToTestScript, Environment.NewLine);

        // create .bat file
        // Path to Bat file
        var mBatFile = Path.Combine(mTestResultsFolder, "test.bat");
        
        try
        {
            using (var file = new StreamWriter(mBatFile))
            {
                file.WriteLine(mCmd);
                file.Flush();
                file.Close();
            }
        }
        catch (Exception ex)
        {
            sbResponse.Append("  Error could not write bat file exception:" + ex.Message);
        }
        var process = new Process
        {
            StartInfo =
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = mBatFile
            }
        };
        var stdOutput = new StringBuilder();
        var stdError = new StringBuilder();
        process.OutputDataReceived += (s, e) => stdOutput.Append(e.Data + Environment.NewLine);
        //process.ErrorDataReceived += (s, e) => stdError.Append(e.Data + Environment.NewLine);

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            var error = process.StandardError.ReadToEnd();
            while (!process.HasExited)
            {
                process.WaitForExit(1000);
            }
            stdError.Append(error);
        }
        catch (Exception ex)
        {
            sbResponse.Append("  Error process exception:" + ex.Message);
            stdError.Append(Environment.NewLine + "  Error Exception:" + ex.Message + Environment.NewLine);
        }
        process.Close();
        // process outout.xml
        var testResult = mTestName + ":" + GetTestResults(mTestResultsFolder);
        if (String.IsNullOrEmpty(testResult))
            testResult = mTestName + ":" + "No test results!";
        try
        {
            var mExeFile = Path.Combine(mTestResultsFolder, "ExecutionResults.txt");
            var mfile = new StreamWriter(mExeFile);
            mfile.WriteLine(testResult);
            mfile.WriteLine("------- StandardOut ----------------");
            mfile.Write(stdOutput.ToString());
            mfile.WriteLine();
            if (stdError.Length != 0)
            {
                mfile.WriteLine("------- StandardError ----------------");
                mfile.Write(stdError.ToString());
                mfile.WriteLine();
            }
            mfile.Flush();
            mfile.Close();
        }
        catch (Exception ex)
        {
            sbResponse.Append("  Error file write exception:" + ex.Message);
        }
        if (!testResult.Contains("Error"))
            ResultSummary = true;

        sbResponse.Replace("testpassed=testpassed", "testpassed='" + ResultSummary + "'");
        sbResponse.Append(testResult);
        sbResponse.Append("</testresult>");
        sbResponse.Append("</serviceresponse>");
        xmlResponse.LoadXml(sbResponse.ToString());
        return xmlResponse;
    }

    protected string CreateDir(string pPath)
    {
        if (Directory.Exists(pPath)) return String.Empty;
        try
        {
            Directory.CreateDirectory(pPath);
            return String.Empty;
        }
        catch (Exception ex)
        {
            return "  Error Could not create " + pPath + " " + ex.Message;
        }
    }

    protected string GetTestResults(string pTestResultsFolder)
    {
        var outputfile = Path.Combine(pTestResultsFolder, OUTPUTFILE);
        if (File.Exists(outputfile) == false)
            return "  Error no output file at " + outputfile;

        try
        {
            var mOutput = new XmlDocument();
            using (var file = new StreamReader(outputfile))
            {
                var content = file.ReadToEnd();
                file.Close();
                mOutput.LoadXml(content);
            }
            XmlNode root = mOutput.DocumentElement;

            if (root == null) 
                return "  Error parsing output.xml, no root node";
            // get script file name
            var mSuiteNode = root.SelectSingleNode("/robot/suite");
            if (mSuiteNode == null)
                return "  Error parsing output.xml, xpath=/robot/suite/status";

            var mStatNode = root.SelectSingleNode("/robot/statistics/suite/stat");
            if (mStatNode == null)
                return "  Error parsing output.xml, xpath=/robot/statistics/suite/stat";

            if (mStatNode.Attributes == null)
                return string.Format("  Error No pass / fail counts {0}", mStatNode.InnerText);

            var resultmsg = string.Format("{0} Passed={1} Failed={2}",
                    mStatNode.InnerText, mStatNode.Attributes["pass"].Value, mStatNode.Attributes["fail"].Value);
            return resultmsg;
        }
        catch (Exception e)
        {
            return "  Error reading output.xml file, exception:" + e.Message;
        }
    }

    protected string StringToHex(String input)
    {
        if (String.IsNullOrEmpty(input)) return string.Empty;
        try
        {
            var values = input.ToCharArray();
            var output = new StringBuilder();
            foreach (var value in values.Select(Convert.ToInt32))
            {
                // Convert the decimal value to a hexadecimal value in string form. 
                output.Append(String.Format("{0:X}", value));
            }
            return output.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    protected string HexToString(String input)
    { // data is encoded in hex, convert it back to a string
        if (String.IsNullOrEmpty(input)) return null;
        try
        {
            var sb = new StringBuilder();
            for (var i = 0; i < input.Length; i += 2)
            {
                var hs = input.Substring(i, 2);
                sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
            }
            return sb.ToString();
        }
        catch
        {
            return null;
        }
    }
}
