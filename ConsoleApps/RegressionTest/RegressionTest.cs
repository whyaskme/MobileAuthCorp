using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Xml;

namespace RegressionTest
{
    class RegressionTest
    {
        private static string pRegTestFolder = "";
        /*
        RegressionTest -sut SystemUnderTest -var VariableName:VariableValue -tf TestFolder
        */
        private static string pRunTime = String.Empty;
        private static int Main(string[] args)
        {
            var pPathToRegressionTest = ConfigurationManager.AppSettings["PathToRegressionTest"];
            var pListTestScriptsPaths = new List<string>();
            var pCommandLineVars = new List<string>();

            pRunTime = DateTime.Now.ToString("MMddyyyyhhmmss");
            if (CreateProcessingFolders() == false) return -1;
            if (args.Any())
            {

                var mProcSUT = false;
                var mProcVars = false;
                var mProcTests = false;
                foreach (var arg in args)
                {
                    if (arg == "-tf")
                    {
                        mProcTests = true;
                    }
                    else if (arg == "-var")
                    {
                        mProcVars = true;
                    }
                    else if (arg == "-sut")
                    {
                        mProcSUT = true;
                    }
                    else if (arg == "-h")
                    {
                        DisplayHelp();
                        return 0;
                    }
                    else
                    {
                        if (mProcSUT)
                        {
                            pCommandLineVars.Add("SUT:" + arg.ToUpper());
                            mProcSUT = false;
                        }
                        else if (mProcVars)
                        {
                            if (!arg.Contains(":"))
                            {
                                Log("Invalid variable format error sb:VariableName:VeriableValue is:" + arg);
                                return -1;
                            }
                            pCommandLineVars.Add(arg);
                            mProcVars = false;
                        }
                        else if (mProcTests)
                        {
                            var mTestSourceFolder = Path.Combine(pPathToRegressionTest, arg);
                            var mFiles = Directory.GetFiles(mTestSourceFolder, "*.txt");
                            if (mFiles.Any())
                            {
                                foreach (var mFile in mFiles)
                                {
                                    pListTestScriptsPaths.Add(mFile);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No Test Script file exists at ");
                                Console.WriteLine(mTestSourceFolder);
                                return -1;
                            }
                        }
                        else
                        {
                            DisplayHelp();
                            return -1;
                        }
                    }
                } // end processing command line arguments
            }
            else
            {
                Console.WriteLine("No command line parameters use -h for help!");
                Console.ReadLine();
                return 0;
            }
            if (pListTestScriptsPaths.Any() == false)
            {
                var mTestSourceFolderList = Directory.GetDirectories(pPathToRegressionTest);
                if (!mTestSourceFolderList.Any())
                {
                    Log("No test source folders in " + mTestSourceFolderList);
                    return -1;
                }
                foreach (var mFolder in mTestSourceFolderList)
                {
                    var mFiles = Directory.GetFiles(mFolder);
                    foreach (var mFile in mFiles)
                    {
                        if (mFile.EndsWith(".txt"))
                        {
                            pListTestScriptsPaths.Add(mFile);
                        }
                    }
                }
                if (!pListTestScriptsPaths.Any())
                {
                    Log("No test script files found " + mTestSourceFolderList);
                    return -1;
                }
            }
            if (pCommandLineVars.Any() == false)
            {
                // set default command line parameters
                pCommandLineVars.Add("SUT:LOCALHOST");
                pCommandLineVars.Add("USER:\"QAUser@mobileauthcorp.com\"");
                pCommandLineVars.Add("PWD:\"QA1234\"");
            }
            foreach (var mTestScript in pListTestScriptsPaths)
            {
                Log("Test script " + Path.GetFileName(mTestScript));
                RunTest(mTestScript, pCommandLineVars);
                ProcessResults(Path.GetFileNameWithoutExtension(mTestScript));
            }
            return 0;
        }

        private static void RunTest(string pTestToRun1, IEnumerable<string> pCommandLineVars)
        {
            var mResultsFolder = Path.Combine(ConfigurationManager.AppSettings["RegressionTestFolder"],"Result");

            Log("Use script file at: " + pTestToRun1);
            var mTextCommandLine = string.Empty;
            foreach (var mVar in pCommandLineVars)
            {
                mTextCommandLine = String.Format("{0} --variable {1}", mTextCommandLine, mVar);
            }
            // add path output directory and path to script
            mTextCommandLine = String.Format("{0} --outputdir \"{1}\" \"{2}\"",
                mTextCommandLine,
                mResultsFolder,
                pTestToRun1);
            try
            {
                Log("pybot" + mTextCommandLine);
                var cmd = Process.Start("pybot", mTextCommandLine);
                if (cmd != null)
                    cmd.WaitForExit();
                Log("Run Complete");
            }
            catch (Exception ex)
            {
                Log("RunTest.Process Error: Could not execute test" + Environment.NewLine + ex.ToString());
            }
        }

        private static void ProcessResults(string pTestToRun1)
        {
            string mResultsStatement;
            var mPF_Folder = "Passed_" + pRunTime;
            var mResultsFolder = Path.Combine(ConfigurationManager.AppSettings["RegressionTestFolder"], "Result");
            if (Directory.Exists(mResultsFolder) == false)
            {
                var msg = "ProcessResults: No results folder at " + mResultsFolder;
                Log(msg);
                WriteSummaryReport(msg);
                return;
            }
            var mOutputFilePath = Path.Combine(mResultsFolder, "output.xml");
            var mOutput = new XmlDocument();
            try
            {
                var mData = File.ReadAllText(mOutputFilePath);
                mOutput.LoadXml(mData);
            }
            catch (Exception ex)
            {
                var msg = "ProcessResults: Exception reading xml file at " + mOutputFilePath +
                    Environment.NewLine + ex.ToString();
                Log(msg);
                WriteSummaryReport(msg);
                return;
            }
            XmlNode root = mOutput.DocumentElement;
            if (root == null)
            {
                const string msg = "ProcessResults: Error parsing output xml, no root node";
                Log(msg);
                WriteSummaryReport(msg);
                return;
            }
            // get script file name
            var mSuiteNode = root.SelectSingleNode("/robot/suite");
            if (mSuiteNode == null)
            {
                const string msg = "ProcessResults: Error parsing output xml, xpath=/robot/suite/status";
                Log(msg);
                WriteSummaryReport(msg);
                return;
            }
            var ScriptFile = "N/A";
            if (mSuiteNode.Attributes != null)
            {
                var mPathToScriptFile = mSuiteNode.Attributes["source"].Value;
                if (String.IsNullOrEmpty(mPathToScriptFile) == false)
                    ScriptFile = Path.GetFileName(mPathToScriptFile);
            }
            Log("Script File=" + ScriptFile);

            // get the run time
            var mSuitestatusNode = root.SelectSingleNode("/robot/suite/status");
            if (mSuitestatusNode == null)
            {
                const string msg = "ProcessResults: Error parsing output xml, xpath=/robot/suite/status";
                Log(msg);
                WriteSummaryReport(msg);
                return;
            }
            var RunDateTime = DateTime.Now;
            if (mSuitestatusNode.Attributes != null)
            {
                var mStartTime = mSuitestatusNode.Attributes["starttime"].Value;
                if (mStartTime != null)
                {
                    try
                    {
                        var mY = mStartTime.Substring(0, 4);
                        var mM = mStartTime.Substring(4, 2);
                        var mD = mStartTime.Substring(6, 2);
                        var mT = mStartTime.Replace(mY + mM + mD, "");
                        RunDateTime = DateTime.Parse(string.Format("{0}/{1}/{2} {3}", mM, mD, mY, mT));
                    }
                    catch
                    {
                        var msg = "ProcessResults: Could not convert starttime [" + mStartTime + "] to DateTime!";
                        Log(msg);
                        WriteSummaryReport(msg);
                        return;
                    }
                }
            }
            Log("RunTime=" + RunDateTime.ToString());

            var mStatNode = root.SelectSingleNode("/robot/statistics/suite/stat");
            if (mStatNode == null)
            {
                const string msg = "ProcessResults: Error parsing output xml, xpath=/robot/statistics/suite/stat";
                Log(msg);
                WriteSummaryReport(msg);
                return;
            }
            if (mStatNode.Attributes == null)
            {
                Log("ProcessResults: Error could not get pass/fail counts from XML.");
                mResultsStatement = string.Format("{0} No pass / fail counts", mStatNode.InnerText);
            }
            else
            {
                var mCountFailed = mStatNode.Attributes["fail"].Value;
                if (mCountFailed != "0")
                    mPF_Folder = "Failed_" + pRunTime;
                else
                    mPF_Folder = "Passed_" + pRunTime;

                mResultsStatement = string.Format("{0} Passed={1} Failed={2}",
                    mStatNode.InnerText, mStatNode.Attributes["pass"].Value, mStatNode.Attributes["fail"].Value);
            }
            Log("ResultsStatement=" + mResultsStatement);
            WriteSummaryReport(mResultsStatement);
            MoveResultsFiles(pTestToRun1, mPF_Folder);
        }

        private static void MoveResultsFiles(string pTestToRun1, string pPF_Folder)
        {
            //--- move results files to final resting place ---------
            /*
             * if fail move results files to failed folder
             * if pass move results files to passed folder
             */
            // Common test results folder
            var mResultsFolder = Path.Combine(ConfigurationManager.AppSettings["RegressionTestFolder"], "Result");
            // create path to results folder
            var mTestName = Path.GetFileNameWithoutExtension(pTestToRun1);
            if (string.IsNullOrEmpty(mTestName)) mTestName = "Result";
            var mToFolder = Path.Combine(ConfigurationManager.AppSettings["RegressionTestFolder"], pPF_Folder, mTestName);
            if (Directory.Exists(mToFolder))
            {
                for (int x = 0; x<9; ++x)
                {
                    var mNewFolder = mToFolder + x;
                    if (!Directory.Exists(mNewFolder))
                    {
                        mToFolder = mNewFolder;
                        break;
                    }
                }
            }
            try
            {
                Directory.Move(mResultsFolder, mToFolder);
            }
            catch (Exception ex)
            {
                Log(" Move Files exception:" + Environment.NewLine + mToFolder + Environment.NewLine + ex.ToString());
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("Command Line format:");
            Console.WriteLine(" RegressionTest -sut SystemUnderTest -var VariableName:VariableValue -tf TestFolder");
            Console.WriteLine("   -sut SystemUnderTest");
            Console.WriteLine("         Example: -sut demo");
            Console.WriteLine("   -var VariableName:VariableValue");
            Console.WriteLine("         Examples: -var USER:BWhite  -var PWD:QA1234 (one varable per -var)");
            Console.WriteLine("   -tf RegressionTestFolder (Optional if not supplied run ALL tests.)");
            Console.WriteLine("         Example: -tf MAC_LogIn (one test folder per -tf)");

            Console.WriteLine();
            Console.WriteLine("Note: Be sure to check your app.config to insure the system under test urls are correct.");
            Console.WriteLine("  They can be found at:");
            Console.WriteLine("  <appSettings>");
            Console.WriteLine("  <add key=\"SystemUnderTestUrl_localhost\" value=\"http://localhost/Admin/Security\" />");
            Console.WriteLine("  <add key=\"SystemUnderTestUrl_demo\" value=\"http://demo.mobileauthcorp.com/Admin/Security\" />");
            Console.WriteLine("  <add key=\"SystemUnderTestUrl_qa\" value=\"http://qa.mobileauthcorp.com/Admin/Security\" />");

            Console.ReadLine();
        }

        private static void WriteSummaryReport(string pSummaryDetails)
        {
            var mCurrentLogFilePath = Path.Combine(pRegTestFolder, "SummaryReport_" + pRunTime + ".txt");
            if (!File.Exists(mCurrentLogFilePath))
            {
                // Create a file to write to. 
                using (var sw = File.CreateText(mCurrentLogFilePath))
                {
                    sw.WriteLine(pSummaryDetails);
                }
            }

            using (var sw = File.AppendText(mCurrentLogFilePath))
            {
                sw.WriteLine(pSummaryDetails);
            }
        }

        private static void Log(string pLogEntry)
        {
            var dt = DateTime.Now;
            var mLogEntry = dt.ToString("MMddyyyyhhmmss") + ", " + pLogEntry;
            var mCurrentLogFilePath = Path.Combine(pRegTestFolder, "LogFile_" +  pRunTime + ".txt");
            if (!File.Exists(mCurrentLogFilePath))
            {
                // Create a file to write to. 
                using (var sw = File.CreateText(mCurrentLogFilePath))
                {
                    sw.WriteLine(mLogEntry);
                }
            }
            else
            {
                using (var sw = File.AppendText(mCurrentLogFilePath))
                {
                    sw.WriteLine(mLogEntry);
                }
            }
            Console.WriteLine(mLogEntry);
        }

        private static bool CreateProcessingFolders()
        {
            // Regression Test Folder
            try
            {
                pRegTestFolder = ConfigurationManager.AppSettings["RegressionTestFolder"];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Configuration Error, Could not get RegressionTestFolder from config!" +
                    ex.ToString());
                return false;
            }
            var mPassedFolder = Path.Combine(pRegTestFolder, "Passed_" + pRunTime);
            var mFailedFolder = Path.Combine(pRegTestFolder, "Failed_" + pRunTime);
            // if it exists 
            if (Directory.Exists(pRegTestFolder))
            {
                if (Directory.Exists(mPassedFolder))
                {
                    Directory.Delete(mPassedFolder, true);
                }
                try
                {
                    Directory.CreateDirectory(mPassedFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not create a folder at " +
                        mPassedFolder + Environment.NewLine + ex.ToString());
                    return false;
                }

                if (Directory.Exists(mFailedFolder))
                {
                    Directory.Delete(mFailedFolder, true);
                }
                try
                {
                    Directory.CreateDirectory(mFailedFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not create a folder at " +
                        mFailedFolder + Environment.NewLine + ex.ToString());
                    return false;
                }
            }
            else
            { // Create the processing folder
                try
                {
                    Directory.CreateDirectory(pRegTestFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not create a folder at " +
                        pRegTestFolder + Environment.NewLine + ex.ToString());
                    return false;
                }
                // create pass fail results sub-folders
                try
                {
                    Directory.CreateDirectory(mPassedFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not create a folder at " +
                        mPassedFolder + Environment.NewLine + ex.ToString());
                    return false;
                }
                try
                {
                    Directory.CreateDirectory(mFailedFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not create a folder at " +
                        mFailedFolder + Environment.NewLine + ex.ToString());
                    return false;
                }
            }
            return true;
        }

    }
}
