using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Net.Mail;
using System.Xml;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using MACSecurity;
using cs = MACServices.Constants.Strings;
using dk = MACServices.Constants.Dictionary.Keys;
using dv = MACServices.Constants.Dictionary.Values;
using cfgcs = MACServices.Constants.WebConfig.ConnectionStringKeys;
using cfg = MACServices.Constants.WebConfig.AppSettingsKeys;
using sr = MACServices.Constants.ServiceResponse;
using mbrsp = MACServices.Constants.MessageBroadcast.ResponseKeys;

using MACOperationalTestLib;
using ot = MACOperationalTestLib.OperationalTestConstants;

namespace OperationalTest
{
    /* This application is started by the system schedular
    Process:
    1. Read the Operational Test Collection
    2. Loop through systems under test list
        2.1. Loop through each test config
            2.1.1 create command line to execuite a test script
            2.1.2 Execute the test
            2.1.3 Process test results
                2.1.3.1 Moves Operational Test result files to temp folder
                2.1.3.2 Opens the “output.xml” file and checks the test results
                2.1.3.3 Archive test results in results collection
            2.1.2 If test results show failures loop through contacts
                2.1.2.1 If contact has semd email set send alert text message
                2.1.2.2 If contact has semd email set or send faliure message
* */

    internal class OperationalTestApp
    {
        public static string mode = ConfigurationManager.AppSettings["Mode"];
        public static string pSystemsUnderTestToIgnore = ConfigurationManager.AppSettings["SystemUnderTestToIgnore"];
        private static MACOperationalTestLib.OperationalTest pOperationalTest;
        private static string pProcessingFolder = "";
        private static string pLogsFolder = "";
        private static string pManualModeMenuAnswer = "";
        private static string pResultsStatements = string.Empty;
        private static bool pHadFailure;

        private static int Main()
        {
            if (CreateDailyProcessingLogFolder() == false) return -1;
            if (CreateProcessingFolder() == false)
            {
                Log("Main Error, Could not Create Processing Folder @" + pProcessingFolder);
                Log("Run aborted!");
                return -1;
            }

            // log start of run
            var mRunTime = DateTime.UtcNow;
            Log("Run Time " + mRunTime.ToString("MMddyyyyhhmmss") + "(UTC)");
            // get the top level collection 'OperationalTest'
            try
            {
                pOperationalTest = GetOperationalTest();
                if (pOperationalTest == null)
                {
                    Log("Error, Could not read the OperationalTest Collection from database!");
                    Log("Run aborted!");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Log("Exception reading OperationalTest Collection from database!"
                    + Environment.NewLine
                    + ex.ToString());
                Log("Run aborted!");
                return -1;
            }
            if (pOperationalTest.SystemsUnderTestList.Count < 1)
            {
                Log("Warning, No System Under Test entries!");
                Log("End Run!");
                return 0;
            }
            // === mode check ====
            if (mode == "manual")
            {
                Log("Manual mode");
                if (DoManualTesting(pOperationalTest))
                {
                    if (!pManualModeMenuAnswer.StartsWith("a"))
                        EnterCommand("Exit");
                    return 0;
                }
            }

            // === Auto mode ===
            Log("Auto mode");

            // Loop through systems under test list
            foreach (var SystemsUnderTest in pOperationalTest.SystemsUnderTestList)
            {
                Log("System under test :" + SystemsUnderTest.SystemName);
                if (!String.IsNullOrEmpty(pSystemsUnderTestToIgnore))
                {
                    if (pSystemsUnderTestToIgnore.Contains(SystemsUnderTest.SystemName))
                    {
                        Log(SystemsUnderTest.SystemName + " is in the ignore list of app.config");
                        continue;
                    }
                }
                pResultsStatements = String.Empty;
                pHadFailure = false;
                if (SystemsUnderTest.TestConfigList.Count < 1)
                {
                    Log("Warning: No tests configured for " + SystemsUnderTest.SystemName);
                    continue;
                }
                // loop through this system under test tests configs
                foreach (var TestConfig in SystemsUnderTest.TestConfigList)
                {
                    var ST = DateTime.Now;
                    var tn = Security.DecodeAndDecrypt(TestConfig.eTestName, cs.DefaultClientId);
                    Log("Start Test: " + tn);
                    
                    // run a test
                    var mResultsFolder = RunTest(SystemsUnderTest, TestConfig);
                    if (String.IsNullOrEmpty(mResultsFolder))
                    {
                        Log("RunTest did not return results folder " + tn);
                        continue;
                    }
                    var runtime = (DateTime.Now - ST).ToString();
                    // process results
                    var mResultsObject = ProcessResults(SystemsUnderTest, TestConfig, mResultsFolder);
                    if (mResultsObject == null)
                    {
                        Log("No results returned from Process Results for test " + tn);
                        continue;
                    }
                    mResultsObject.TestRunTime = runtime;
                    SaveResultsDocument(mResultsObject);
                    Log("End Test: " + tn + " Runtime: " + runtime);
                }
                // did one of the tests fail
                if (pHadFailure)
                { // had a failure send failure to contacts
                    NotifyContactsOfFailure(pOperationalTest.TestSystemName, SystemsUnderTest);
                }
            }
            Log("End Run " + pResultsStatements);
            return 0;
        }

        #region Run methods
        private static string RunTest(OperationalTestSystemUnderTest pSystemsUnderTest, OperationalTestConfig TestConfig)
        {
            // check if test script exists
            var PathToTestScript = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", 
                Security.DecodeAndDecrypt(TestConfig.eTestScript, cs.DefaultClientId));
            if (!File.Exists(PathToTestScript))
            {
                Log("RunTest Error: No test script file at: " + PathToTestScript);
                return null;
            }
            Log("Use script file at: " + PathToTestScript);
            var mResultsFolder = CreateResultsFolder(Security.DecodeAndDecrypt(TestConfig.eTestName, cs.DefaultClientId));
            if (String.IsNullOrEmpty(mResultsFolder))
            {
                Log("RunTest Error: Could not create Results folder!");
                return null; 
            }
            // Create command line to run test
            var mCommandLineVars = String.Format(" --variable \"SUT:{0}\"", pSystemsUnderTest.SystemName);
            var mVar1 = Security.DecodeAndDecrypt(TestConfig.eTestCommandLineVariables, cs.DefaultClientId);
            var mVars = mVar1.Split('~');
            foreach (var mVar in mVars)
            {
                if (String.IsNullOrEmpty(mVar)) continue;
                mCommandLineVars += String.Format(" --variable \"{0}\"", mVar);
            }

            var mTextCommandLine = String.Format("{0} --outputdir \"{1}\" \"{2}\"",
                mCommandLineVars,
                mResultsFolder,
                PathToTestScript);
            try
            {
                Log(ot.TestApp + mTextCommandLine);
                var cmd = Process.Start(ot.TestApp, mTextCommandLine);
                if (cmd != null) cmd.WaitForExit();
                Log("Run Complete");

            }
            catch (Exception ex)
            {
                Log("RunTest.Process Error: Could not execute test" + Environment.NewLine + ex.ToString());
                return null;
            }
            return mResultsFolder;
        }

        private static OperationalTestResults ProcessResults(OperationalTestSystemUnderTest SystemsUnderTest,
            OperationalTestConfig TestConfig, string pResultsFolder)
        {
            var mResult = new OperationalTestResults {SystemUnderTestName = SystemsUnderTest.SystemName};
            Log("System Under Test Name =" + mResult.SystemUnderTestName);

            mResult.TestName = Security.DecodeAndDecrypt(TestConfig.eTestName, cs.DefaultClientId);
            Log("Test Name=" + mResult.TestName);

            if (Directory.Exists(pResultsFolder) == false)
            {
                Log("ProcessResults: No results folder at " + pResultsFolder);
                return null;
            }

            var mOutput = new XmlDocument();
            try
            {
                var mData = File.ReadAllText(Path.Combine(pResultsFolder, ot.resultsOutputFileName));
                mOutput.LoadXml(mData);
            }
            catch (Exception ex)
            {
                Log("ProcessResults: Exception reading xml file at " + Path.Combine(pResultsFolder, ot.resultsOutputFileName) +
                    Environment.NewLine + ex.ToString());
                return null;
            }
            XmlNode root = mOutput.DocumentElement;
            if (root == null)
            {
                Log("ProcessResults: Error parsing output xml, no root node");
                return null;
            }
            // get script file name
            var mSuiteNode = root.SelectSingleNode("/robot/suite");
            if (mSuiteNode == null)
            {
                Log("ProcessResults: Error parsing output xml, xpath=/robot/suite/status");
                return null;
            }
            mResult.ScriptFile = "N/A";
            if (mSuiteNode.Attributes != null)
            {
                var mPathToScriptFile = mSuiteNode.Attributes["source"].Value;
                if (String.IsNullOrEmpty(mPathToScriptFile) == false)
                    mResult.ScriptFile = Path.GetFileName(mPathToScriptFile);
            }
            Log("Script File=" + mResult.ScriptFile);

            // get the run time
            var mSuitestatusNode = root.SelectSingleNode("/robot/suite/status");
            if (mSuitestatusNode == null)
            {
                Log("ProcessResults: Error parsing output xml, xpath=/robot/suite/status");
                return null;
            }

            mResult.RunDateTime = DateTime.Now;
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
                        mResult.RunDateTime = DateTime.Parse(string.Format("{0}/{1}/{2} {3}", mM, mD, mY, mT));
                    }
                    catch
                    {
                        Log("ProcessResults: Could not convert starttime [" + mStartTime + "] to DateTime!");
                    }
                }
            }
            Log("RunTime=" + mResult.RunDateTime.ToString());

            mResult.ResultsStatement = "N/A";
            var mStatNode = root.SelectSingleNode("/robot/statistics/suite/stat");
            if (mStatNode != null)
            {
                if (mStatNode.Attributes == null)
                {
                    mResult.ResultsStatement += string.Format("  Error No pass / fail counts {0}", mStatNode.InnerText);
                }
                else
                {
                    mResult.ResultsStatement = string.Format("{0} Passed={1} Failed={2}",
                        mStatNode.InnerText, mStatNode.Attributes["pass"].Value, mStatNode.Attributes["fail"].Value);
                }
            }
            Log("Results Statement=" + mResult.ResultsStatement);
            AddToResultsStatements(mResult.ResultsStatement);

            var mResultFiles = Directory.GetFiles(pResultsFolder);
            foreach (var mResultFile in mResultFiles)
            {
                var mFile = new OperationalTestResultFile();
                var mFileName = mFile.Name = Path.GetFileName(mResultFile);
                // remove selenium from start of file name if it exists
                if (String.IsNullOrEmpty(mFileName) == false)
                    if (mFileName.StartsWith("selenium-"))
                        mFile.Name = mFileName.Replace("selenium-", "");
                // file type less the .
                var mType = mFile.Type = Path.GetExtension(mResultFile);
                if (!String.IsNullOrEmpty(mType))
                    mFile.Type = mType.Replace(".", "");
                // read the file into content
                try
                {
                    Log("Adding Results File[" + mFile.Name + "] to collection");
                    var filecontent = File.ReadAllText(mResultFile);
                    mFile.eContent = Security.EncryptAndEncode(filecontent, cs.DefaultClientId);
                    mResult.ResultFileList.Add(mFile);
                }
                catch (Exception ex)
                {
                    Log("ProcessResults: Exception adding results file " + mResultFile +
                        Environment.NewLine + ex.ToString());
                    mFile.eContent = null;
                    mResult.ResultFileList.Add(mFile);
                }
            }
            // ========== clear processing folder ========================================
            Log("Delete results folder");
            Directory.Delete(pResultsFolder, true);
            return mResult;
        }

        private static bool NotifyContactsOfFailure(string pTestSystemName, OperationalTestSystemUnderTest pSystemUnderTest)
        {
            foreach (var mContact in pSystemUnderTest.ContactList)
            {
                var fn = Security.DecodeAndDecrypt(mContact.eFirstName, cs.DefaultClientId);
                var ln = Security.DecodeAndDecrypt(mContact.eLastName, cs.DefaultClientId);
                var pn = Security.DecodeAndDecrypt(mContact.ePhoneNumber, cs.DefaultClientId);
                var em = Security.DecodeAndDecrypt(mContact.eEmailAddress, cs.DefaultClientId);

                if (mContact.SendSMS)
                {
                    var message = pOperationalTest.SmsSettings.TextMessageTemplate.Replace(ot.ReplacementKeys.SUT, pSystemUnderTest.SystemName);
                    Log("Send Text Message to " + fn + " " + ln);
                    Log(message);
                    SendTextMessage(pn, message);
                }
                if (mContact.SendEmail)
                {
                    Log("Send email to " + fn + " " + ln);
                    var subject = pOperationalTest.EmailSettings.EmailSubject.Replace(ot.ReplacementKeys.SUT, pSystemUnderTest.SystemName);
                    Log(subject);
                    var EmailBody = fn + ",|"
                                    + pOperationalTest.EmailSettings.EmailBodyTemplate.Replace(ot.ReplacementKeys.RESULTSSTATEMENT, pResultsStatements).Replace(ot.ReplacementKeys.TESTSYSTEM, pTestSystemName);
                    SendEmail(subject, EmailBody, em);
                }
            }
            return true;
        }
        #endregion

        #region Create folders

        private static string CreateResultsFolder(String pTestName)
        {
            var mResultsFolder = Path.Combine(pProcessingFolder, pTestName + "_results");
            // Delete Results folder and everything in it
            if (Directory.Exists(mResultsFolder))
            {
                try
                {
                    var dir = new DirectoryInfo(mResultsFolder);
                    dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
                    dir.Delete(true);
                }
                catch (IOException ex)
                {
                    Log("Error: Could not delete " + mResultsFolder + " and content " + Environment.NewLine + ex.ToString());
                    Log("Run aborted!");
                    return null;
                }
            }
            // Create the results folder
            try
            {
                Directory.CreateDirectory(mResultsFolder);
            }
            catch (Exception ex)
            {
                Log("RunTest Error: Could not create a folder at " +
                    mResultsFolder + Environment.NewLine + ex.ToString());
                Log("Test aborted!");
                return null;
            }
            return mResultsFolder;
        }

        private static bool CreateProcessingFolder()
        {
            try
            {
                pProcessingFolder = ConfigurationManager.AppSettings[ot.Cfg.ProcessingFolder];
            }
            catch (Exception ex)
            {
                Log("Error: Configuration Error, Could not get " + ot.Cfg.ProcessingFolder + " from config! " +
                    ex.ToString());
                Log("Run aborted!");
                return false;
            }

            // Create the processing folder
            try
            {
                Directory.CreateDirectory(pProcessingFolder);
            }
            catch (Exception ex)
            {
                Log("Error: Could not create a folder at " +
                    pProcessingFolder + Environment.NewLine + ex.ToString());
                Log("Run aborted!");
                return false;
            }
            return true;
        }

        private static bool CreateDailyProcessingLogFolder()
        {
            // todo: keep processing log file purge after 30 days.
            try
            {
                pLogsFolder = ConfigurationManager.AppSettings[ot.Cfg.ProcessingLogsFolder];
            }
            catch (Exception ex)
            {
                Log("Error: Configuration Error, Could not get " + ot.Cfg.ProcessingLogsFolder + " from config! " +
                    ex.ToString());
                Log("Run aborted!");
                return false;
            }
            if (Directory.Exists(pLogsFolder))
            { // delete any log files older that 30 days
                var mFilePaths = Directory.GetFiles(pLogsFolder);
                foreach (var mFilePath in mFilePaths)
                {
                    var fileCreatedDate = File.GetCreationTime(mFilePath);
                    if (fileCreatedDate < DateTime.Now.AddDays(-30))
                    {
                        Log("Delete log file " + Path.GetFileName(mFilePath));
                        File.Delete(mFilePath);
                    }
                }
                return true;
            }
            // Create the processing folder
            try
            {
                Directory.CreateDirectory(pLogsFolder);
            }
            catch (Exception ex)
            {
                Log("Error: Could not create the Processing Logs folder at " +
                    pLogsFolder + Environment.NewLine + ex.ToString());
                Log("Run aborted!");
                return false;
            }
            return true;
        }

        #endregion

        #region Helper Methods & Logging
        private static void AddToResultsStatements(string pStatement)
        {
            if (String.IsNullOrEmpty(pResultsStatements))
                pResultsStatements = pStatement;
            else
                pResultsStatements = String.Format("{0}|{1}", pResultsStatements, pStatement);
        }

        private static void Log(string pLogEntry)
        {
            var dt = DateTime.Now;
            var mLogEntry = dt.ToString("MMddyyyyhhmmss") + ", " + pLogEntry;
            var mProcessingLogFile = "LogFile_" + dt.ToString("MMddyy") + ".txt";
            if (!String.IsNullOrEmpty(mode))
            {
                if (mode == "manual")
                {
                    Console.WriteLine("Log: " + mLogEntry);
                }
            }
            var mCurrentLogFilePath = Path.Combine(pLogsFolder, mProcessingLogFile);
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
        }

        #endregion

        #region Database Methods

        public static bool SaveResultsDocument(OperationalTestResults pResult)
        {
            var mongoClient =
                new MongoClient(ConfigurationManager.ConnectionStrings[cfgcs.OperationalTestServer].ConnectionString);
            var server = mongoClient.GetServer();
            var db = server.GetDatabase(ConfigurationManager.AppSettings[cfg.MongoDbOperationalTestDBName]);
            try
            {
                var mCollection = db.GetCollection("OperationalTestResults");
                mCollection.Insert(pResult);
                return true;
            }
            catch (Exception ex)
            {
                Log("SaveResultsDocument: Exception: " + ex.Message + Environment.NewLine + ex.ToString());
                return false;
            }
            finally
            {
                server.Disconnect();
            }
        }

        public static MACOperationalTestLib.OperationalTest GetOperationalTest()
        {
            var mongoClient =
                new MongoClient(ConfigurationManager.ConnectionStrings[cfgcs.OperationalTestServer].ConnectionString);
            var server = mongoClient.GetServer();
            var db = server.GetDatabase(ConfigurationManager.AppSettings[cfg.MongoDbOperationalTestDBName]);
            try
            {
                var query = Query.EQ("_t", ot.OperationalTestCollectionName);
                var mongoCollection = db.GetCollection(ot.OperationalTestCollectionName);
                return mongoCollection.FindOneAs<MACOperationalTestLib.OperationalTest>(query);
            }
            catch (Exception ex)
            {
                Log("GetOperationalTest: Exception: " + ex.Message + Environment.NewLine + ex.ToString());
                return null;
            }
            finally
            {
                server.Disconnect();
            }
        }

        public static bool UpdateOperationalTest(MACOperationalTestLib.OperationalTest pOperationalTestObject)
        {
            var mongoClient =
                new MongoClient(ConfigurationManager.ConnectionStrings[cfgcs.OperationalTestServer].ConnectionString);
            var server = mongoClient.GetServer();
            var db = server.GetDatabase(ConfigurationManager.AppSettings[cfg.MongoDbOperationalTestDBName]);
            try
            {
                var mongoCollection = db.GetCollection(ot.OperationalTestCollectionName);
                //var query = Query.EQ("_id", pOperationalTestObject._id);
                //var sortBy = SortBy.Descending("_id");

                mongoCollection.Save(pOperationalTestObject);

                //mongoCollection.FindAndRemove(query, sortBy);
                //mongoCollection.Insert(pOperationalTestObject);
                return true;
            }
            catch (Exception ex)
            {
                Log("UpdateOperationalTest: Exception: " + ex.Message + Environment.NewLine + ex.ToString());
                return false;
            }
            finally
            {
                server.Disconnect();
            }
        }


        #endregion

        #region Manual mode methods
        // Manual mode
        private static bool DoManualTesting(MACOperationalTestLib.OperationalTest pOptTest)
        {
            var SystemUnderTest4ManuelMode = ConfigurationManager.AppSettings["SystemUnderTest4ManuelMode"];
            if (String.IsNullOrEmpty(SystemUnderTest4ManuelMode))
            {
                Log("DoManualTesting.AppConfig: missing SystemUnderTest4ManuelMode key!");
                return true;
            }
            var TestConfigName4ManuelMode = ConfigurationManager.AppSettings["TestConfigName4ManuelMode"];
            if (String.IsNullOrEmpty(TestConfigName4ManuelMode))
            {
                Log("DoManualTesting.AppConfig: missing TestConfigName4ManuelMode key!");
                return true;
            }

            // Manual mode menu, get valid resounse
            while (true)
            {
                var command = EnterCommand(
                        "Enter:" + Environment.NewLine +
                        "   A=Auto run all configured tests, " + Environment.NewLine +
                        "   R=Run Test, " + Environment.NewLine +
                        "   P=Process 'Passed' Test Results, " + Environment.NewLine +
                        "   F=Process 'Failed' Test Results, " + Environment.NewLine +
                        "   S=Send Text Message, " + Environment.NewLine +
                        "   E=Send Email Message, " + Environment.NewLine +
                        "   Q=Quit" + Environment.NewLine +
                        "Command");
                pManualModeMenuAnswer = command.ToLower();
                if (pManualModeMenuAnswer.StartsWith("q"))
                {
                    Log("Menu response:" + command + "=Quit");
                    return true;
                }
                if (pManualModeMenuAnswer.StartsWith("a"))
                {
                    Log("Menu response:" + command + "=Auto");
                    return false;
                }
                if (pManualModeMenuAnswer.StartsWith("s"))
                {
                    Log("Menu response:" + command + "=Send Text Message");
                    SendTextMessage("4802684076", "test message");
                    return true;
                }
                if (pManualModeMenuAnswer.StartsWith("e"))
                {
                    Log("Menu response:" + command + "=Send Email Message");
                    SendEmail("Operational Test", "Test", "tdavis@mobileauthcorp.com");
                    return true;
                }
                Log("Menu response:" + command);
                // get the Operational Test OperationalTestSystemUnderTest and OperationalTestConfig objects by name
                OperationalTestSystemUnderTest mSystemUnderTest = null;
                OperationalTestConfig mTestConfig = null;
                foreach (var mST in pOptTest.SystemsUnderTestList)
                {
                    if (mST.SystemName == SystemUnderTest4ManuelMode)
                    {
                        pResultsStatements = String.Empty;
                        pHadFailure = false;
                        Log("System Under Test=" + mST.SystemName);
                        mSystemUnderTest = mST;
                        foreach (var mTest in mST.TestConfigList)
                        {
                            var tn = Security.DecodeAndDecrypt(mTest.eTestName, cs.DefaultClientId);
                            if (tn == TestConfigName4ManuelMode)
                            {
                                Log("Test Configuration=" + tn);
                                mTestConfig = mTest;
                                break;
                            }
                        }
                        break;
                    }
                }
                if (mSystemUnderTest == null)
                {
                    Log("DoManualTesting: Could not find the System Under Test [" + SystemUnderTest4ManuelMode + "]");
                    return true;
                }
                if (mTestConfig == null)
                {
                    Log("DoManualTesting: Could not find Test Config named[" + TestConfigName4ManuelMode + "]");
                    return true;
                }
                //-------- have system under test object and test object -------------
                if (pManualModeMenuAnswer.StartsWith("r"))
                { // run the test
                    var mResultsFolder = RunTest(mSystemUnderTest, mTestConfig);
                    if (AskProcessResultsForThisTest())
                    {
                        var mResultsObject = ProcessResults(mSystemUnderTest, mTestConfig, mResultsFolder);
                        if (AskSaveResults())
                        {
                            SaveResultsDocument(mResultsObject);
                            if (pHadFailure)
                            {
                                // ask to send motifications
                                if (AskSendFailureAlerts())
                                {
                                    return NotifyContactsOfFailure(pOptTest.TestSystemName, mSystemUnderTest);
                                }
                            }
                        }
                    }
                    return true;
                } 
                // menu selection for use canned pass or fail results
                if (pManualModeMenuAnswer.StartsWith("p") || pManualModeMenuAnswer.StartsWith("f"))
                { // use canned pass or fail results
                    string mCannedTestResultsFolderName;
                    if (pManualModeMenuAnswer.StartsWith("p"))
                    {
                        Log("CreateTestResults Passed");
                        mCannedTestResultsFolderName = "PassedResults";
                    }
                    else
                    {
                        Log("CreateTestResults Failed");
                        mCannedTestResultsFolderName = "FailedResults";
                    }
                    // Copy test results to processing folder
                    var mResultsFolder = CreateResultsFolder(mCannedTestResultsFolderName);
                    if (String.IsNullOrEmpty(mResultsFolder))
                    {
                        Log("DoManualTesting: Could not create Results folder!");
                        return true;
                    }
                    var mFromPath = Path.Combine(Directory.GetCurrentDirectory(), "TestResultsFolder", mCannedTestResultsFolderName);
                    if (Directory.Exists(mFromPath) == false)
                    {
                        Log("DoManualTesting: Could not find From folder @" + mFromPath);
                        return true;
                    }
                    var mTestFiles = Directory.GetFiles(mFromPath);
                    foreach (var mTestFile in mTestFiles)
                    {
                        var mFileName = Path.GetFileName(mTestFile);
                        if (String.IsNullOrEmpty(mFileName) == false)
                        {
                            if (mFileName.EndsWith(".txt"))
                                mFileName = mFileName.Replace(".txt", "");
                            File.Copy(mTestFile, Path.Combine(mResultsFolder, mFileName));
                        }
                    }
                    var mResultsObject = ProcessResults(mSystemUnderTest, mTestConfig, mResultsFolder);
                    if (AskSaveResults())
                    {
                        SaveResultsDocument(mResultsObject);
                        if (pHadFailure)
                        {
                            // ask to send motifications
                            if (AskSendFailureAlerts())
                            {
                                return NotifyContactsOfFailure(pOptTest.TestSystemName, mSystemUnderTest);
                            }
                        }
                    }
                    return true;
                }
                Console.WriteLine("What?");
            }
        }

        private static bool AskProcessResultsForThisTest()
        {
            if (!String.IsNullOrEmpty(mode))
            {
                if (mode == "manual")
                {
                    if (!pManualModeMenuAnswer.StartsWith("a"))
                    {
                        while (true)
                        {
                            var answer = EnterCommand("Process Result Files?");
                            if (answer.StartsWith("Y")) return true;
                            if (answer.StartsWith("y")) return true;
                            if (answer.StartsWith("N")) return false;
                            if (answer.StartsWith("n")) return false;
                        }
                    }
                }
            }
            return true;
        }

        private static bool AskSaveResults()
        {
            if (!String.IsNullOrEmpty(mode))
            {
                if (mode == "manual")
                {
                    if (!pManualModeMenuAnswer.StartsWith("a"))
                    {
                        var answer = EnterCommand("Add the results to the System Under Test?");
                        if (answer.StartsWith("Y")) return true;
                        if (answer.StartsWith("y")) return true;
                        if (answer.StartsWith("N")) return false;
                        if (answer.StartsWith("n")) return false;
                    }
                }
            }
            return true;
        }

        private static bool AskSendFailureAlerts()
        {
            if (!String.IsNullOrEmpty(mode))
            {
                if (mode == "manual")
                {
                    if (!pManualModeMenuAnswer.StartsWith("a"))
                    {
                        while (true)
                        {
                            var answer = EnterCommand("Send failure alerts?");
                            if (answer.StartsWith("Y")) return true;
                            if (answer.StartsWith("y")) return true;
                            if (answer.StartsWith("N")) return false;
                            if (answer.StartsWith("n")) return false;
                        }
                    }
                }
            }
            return true;
            
        }

        private static string EnterCommand(string pQuestion)
        {
            Console.Write(pQuestion + "? ");
            while (true)
            {
                var answer = Console.ReadLine();
                if (!String.IsNullOrEmpty(answer))
                {
                    return answer;
                }
            }
        }

        #endregion

        #region Send Message to contacts

        public static bool SendTextMessage(string pNumber, string pMessage)
        {
            if (pOperationalTest == null)
            {
                Log("SendTextMessage.Error: Operational Test object null!");
                return false;
            }
            var sendmode = ConfigurationManager.AppSettings["SendMode"];
            if ((String.IsNullOrEmpty(sendmode)) || (sendmode == "NoSend"))
            {
               Log("Notifications No send set!");
                return false;
            }
            Log("Url:" + pOperationalTest.SmsSettings.URL + " BatchId:" + Security.DecodeAndDecrypt(pOperationalTest.SmsSettings.eBatchId, cs.DefaultClientId));

            var MyDateTime = DateTime.UtcNow.ToString();
            var stringToSign = "POST" + "\n\n\n" + MyDateTime + "\n\n\n";
            // private key from Sid properity
            var signature = HmacSha1SignRequest(Security.DecodeAndDecrypt(pOperationalTest.SmsSettings.eSid, cs.DefaultClientId), stringToSign);
            var mTextMessage = pOperationalTest.TestSystemName + Environment.NewLine + pMessage.Replace("|", Environment.NewLine);
            var sbRestRequest = new StringBuilder();
            // batch id determains the Short Code
            sbRestRequest.Append("inpBatchId=" + Security.DecodeAndDecrypt(pOperationalTest.SmsSettings.eBatchId, cs.DefaultClientId));
            // Mobile Phone Number
            sbRestRequest.Append("&inpContactString=" + pNumber);
            sbRestRequest.Append("&inpContactTypeId=3");
            // The test message parts A and B
            sbRestRequest.Append("&inpA=" + Uri.EscapeUriString(mTextMessage));
            sbRestRequest.Append("&inpB=" + Uri.EscapeUriString(" Msg freq depends on user. HELP for help or STOP to opt-out. Msg and Data rates may apply."));
            //Time zone required to get correct error response on invalid phone numbers
            sbRestRequest.Append("&inpTimeZone=Mountain");
            var dataStream = Encoding.UTF8.GetBytes(sbRestRequest.ToString());
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(pOperationalTest.SmsSettings.URL);
                myHttpWebRequest.Method = "POST";
                // Set the content type of the data being posted.
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.ContentLength = dataStream.Length;
                myHttpWebRequest.Headers["datetime"] = MyDateTime;
                var at = Security.DecodeAndDecrypt(pOperationalTest.SmsSettings.eAuthToken, cs.DefaultClientId);
                if (string.IsNullOrEmpty(at))
                {
                    Log("Error: ========== MB AuthToken can not be null! =======================");
                    return false;
                }
                
                myHttpWebRequest.Headers["Authorization"] = Security.DecodeAndDecrypt(pOperationalTest.SmsSettings.eAuthToken, cs.DefaultClientId) + ":" + signature;
                // request XML response
                myHttpWebRequest.Accept = "application/xml";
                var newStream = myHttpWebRequest.GetRequestStream();
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();

                string mResponseData;
                using (var response = (HttpWebResponse) myHttpWebRequest.GetResponse())
                {
                    var header = response.GetResponseStream();
                    if (header == null)
                    {
                        return false;
                    }
                    var encode = Encoding.GetEncoding("utf-8");
                    var readStream = new StreamReader(header, encode);
                    mResponseData = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(mResponseData);
                var elemList = xmlDoc.GetElementsByTagName(mbrsp.BLOCKEDBYDNC);
                var mBLOCKEDBYDNCValue = elemList[0].InnerXml;
                elemList = xmlDoc.GetElementsByTagName(mbrsp.SMSINIT);
                var mSMSINITValue = elemList[0].InnerXml;
                elemList = xmlDoc.GetElementsByTagName(mbrsp.MESSAGE);
                var mMESSAGEValue = elemList[0].InnerXml;
                elemList = xmlDoc.GetElementsByTagName(mbrsp.ERRMESSAGE);
                var mERRMESSAGEValue = elemList[0].InnerXml;
                elemList = xmlDoc.GetElementsByTagName(mbrsp.SMSDEST);
                var mSHORTCODE = elemList[0].InnerXml;

                if (!String.IsNullOrEmpty(mMESSAGEValue))
                    Log("SMS response message:" + mERRMESSAGEValue);
                if (mBLOCKEDBYDNCValue == "1")
                    Log("Sms Error: Error: Not sent, Blocked user replied '" + sr.STOP + "' (" + sr.FromNumber + "=" + mSHORTCODE + ")");
                else if (mSMSINITValue == "0")
                    Log("SMS Error: Not sent (" + mERRMESSAGEValue + ")");
                else
                    Log("SMS Message sent");
                return true;
            }
            catch (Exception ex)
            {
                Log("SMS Exception:" + ex.ToString());
                return false;
            }
        }

        protected static string HmacSha1SignRequest(string privateKey, string valueToHash)
        {
            var encoding = new ASCIIEncoding();

            var keyByte = encoding.GetBytes(privateKey);
            var hmacsha1 = new HMACSHA1(keyByte);

            var messageBytes = encoding.GetBytes(valueToHash);
            var hashmessage = hmacsha1.ComputeHash(messageBytes);
            var hashedValue = Convert.ToBase64String(hashmessage); // convert to base64

            return hashedValue;
        }

        private static void SendEmail(string pSubject, string pBody, string pToAddresses)
        {
            if (pOperationalTest == null)
            {
                Log("SendEmail.Error: Operational Test object null!");
                return;
            }
            var sendmode = ConfigurationManager.AppSettings["SendMode"];
            if ((String.IsNullOrEmpty(sendmode)) || (sendmode == "NoSend"))
            {
                Log("Notifications No send set!");
                return;
            }
            var eh = Security.DecodeAndDecrypt(pOperationalTest.EmailSettings.eEmailHost, cs.DefaultClientId);
            Log("Email Settings: Host[" + eh + "] Port[" + pOperationalTest.EmailSettings.EmailPort + "]");
            try
            {
                var mFromAddress = pOperationalTest.EmailSettings.EmailFromAddress;
                var mail = new MailMessage { From = new MailAddress(mFromAddress) };
                mail.To.Add(pToAddresses);
                mail.Subject = pSubject;
                mail.IsBodyHtml = true;
                mail.Body = pBody.Replace("|", "<br />");
                mail.Priority = MailPriority.High;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                // setup Smtp Client
                var smtp = new SmtpClient
                {
                    Port = pOperationalTest.EmailSettings.EmailPort,
                    Host = eh,
                    EnableSsl = pOperationalTest.EmailSettings.EmailEnableSsl,
                    UseDefaultCredentials = pOperationalTest.EmailSettings.EmailUseDefaultCredentials,
                    Credentials = new NetworkCredential(
                        Security.DecodeAndDecrypt(pOperationalTest.EmailSettings.eEmailLoginUserName, cs.DefaultClientId), 
                        Security.DecodeAndDecrypt(pOperationalTest.EmailSettings.eEmailLoginPassword, cs.DefaultClientId)),
                        DeliveryMethod = SmtpDeliveryMethod.Network
                };
                smtp.Send(mail);
                Log("Email send.");
            }
            catch (Exception ex)
            {
                Log("Error: Send email failed: " + Environment.NewLine + ex.ToString());
            }
        }
        #endregion

    }
}
