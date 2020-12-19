using System;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Net;
using System.Net.Mail;

namespace ResultsProcessor
{
    /* Process:
        1)	Scheduler runs Operational Test Suite from command line
        2)	Scheduler runs Results Processing application
            a.	Results Processing application does the following:
                i.	Moves Operational Test result files to temp folder
                ii.	Opens the “output.xml” file and checks the test results
                    1.	If test results show failures
            a.	Send email to list of recipients 
                i.	Email shall contain the test results 
                ii.	Results files as attachments
                iii.	Archive results files in database
                    1.	Sub- collection name shall contain
                        a.	Operational Test + Test system name
                        b.	Under test System Name
                        c.	UTC Date + Time.
    * */


    class ResultsProcessor
    {
        public static string LogFile = ConfigurationManager.AppSettings["ProcessingLogFile"] + ".txt";

        private static void Main()
        {
            var resultsOutputFileName = "output.xml";
            var resultsReportFileName = "report.html";
            var resultsLogFileName = "log.html";
            if (String.IsNullOrEmpty(LogFile))
            {
                Console.WriteLine("No log file in configuration!");
                return;
            }
            string SUT_AdminEmail;
            string SystemUnderTestName;
            string SUT_ResultsFolder;
            string ArchiveFolder;
            var TestSystemName = "";
            string ProcessingFolder;
            var cfg = "";
            try
            {
                cfg = "TestSystemName";
                TestSystemName = ConfigurationManager.AppSettings[cfg];
                cfg = "ProcessingFolder";
                ProcessingFolder = ConfigurationManager.AppSettings[cfg];
                cfg = "ArchiveFolder";
                ArchiveFolder = ConfigurationManager.AppSettings[cfg];
                cfg = "SystemUnderTest";
                SystemUnderTestName = ConfigurationManager.AppSettings[cfg].ToString();
                cfg = "SUT_ResultsFolder";
                SUT_ResultsFolder = ConfigurationManager.AppSettings[cfg].ToString();
                cfg = "SUT_AdminEmail";
                SUT_AdminEmail = ConfigurationManager.AppSettings[cfg].ToString();
            }
            catch (Exception ex)
            {
                Log("Error: Configuration Error, Could not get " + cfg + " from web.config! " + ex.ToString());
                return;
            }
            // Check if processing folder exists
            if (!Directory.Exists(ProcessingFolder))
            {
                try
                {
                    Directory.CreateDirectory(ProcessingFolder);
                }
                catch (Exception ex)
                {
                    Log("Error: Could not create the processing folder at " + ProcessingFolder + Environment.NewLine +
                        ex.ToString());
                    return;
                }
            }
            // Check if processing folder exists
            if (!Directory.Exists(ArchiveFolder))
            {
                try
                {
                    Directory.CreateDirectory(ArchiveFolder);
                }
                catch (Exception ex)
                {
                    Log("Error: Could not create the Archive folder at " + ArchiveFolder + Environment.NewLine +
                        ex.ToString());
                    return;
                }
            }
            // file in processing folder have to be processed first
            if (!File.Exists(Path.Combine(ProcessingFolder, resultsOutputFileName)))
            {
                // Check if system under test's results folder exists
                if (!Directory.Exists(SUT_ResultsFolder))
                {
                    Log("Error: System under test's results folder does not exists!");
                    return;
                }
                Directory.Move(SUT_ResultsFolder, ProcessingFolder);

                //// move the results files to the processing folder
                //File.Move(Path.Combine(SUT_ResultsFolder, resultsOutputFileName),
                //    Path.Combine(ProcessingFolder, resultsOutputFileName));
                //File.Move(Path.Combine(SUT_ResultsFolder, resultsReportFileName),
                //    Path.Combine(ProcessingFolder, resultsReportFileName));
                //File.Move(Path.Combine(SUT_ResultsFolder, resultsLogFileName),
                //    Path.Combine(ProcessingFolder, resultsLogFileName));
            }


            // Check if must have files exists in processing folder
            if (!File.Exists(Path.Combine(ProcessingFolder, resultsOutputFileName)))
            {
                Log("Error: rocessing folder does not contain the " + resultsOutputFileName + " file!");
                return;
            }
            // Check if system under test's results folder contains result files
            if (!File.Exists(Path.Combine(ProcessingFolder, resultsReportFileName)))
            {
                Log("Error: rocessing folder does not contain the " + resultsReportFileName + " file!");
                return;
            }
            // Check if system under test's results folder contains result files
            if (!File.Exists(Path.Combine(ProcessingFolder, resultsLogFileName)))
            {
                Log("Error: Processing folder does not contain the " + resultsLogFileName + " file!");
                return;
            }

            var mOutput = new XmlDocument();
            var mData = File.ReadAllText(Path.Combine(ProcessingFolder, resultsOutputFileName));
            mOutput.LoadXml(mData);
            var test_node = mOutput.SelectSingleNode("robot");
            if (test_node == null)
            {
                Log("Error: No root node in " + resultsOutputFileName + " file!");
                return;
            }
            if (test_node.Attributes == null)
            {
                Log("Error: No root node in " + resultsOutputFileName + " file!");
                return;
            }
            var dt_Generated = test_node.Attributes["generated"].Value;

            var doc_element = mOutput.SelectSingleNode("robot/suite/doc");
            if (doc_element == null)
            {
                Log("Error: No doc node in " + resultsOutputFileName + " file!");
                return;
            }
            var status_node = mOutput.SelectSingleNode("robot/statistics/total/stat");
            if (status_node == null)
            {
                Log("Error: No status node in " + resultsOutputFileName + " file!");
                return;
            }
            if (status_node.Attributes == null)
            {
                Log("Error: No status node in " + resultsOutputFileName + " file has no Attributes!");
                return;
            }
            var passed = status_node.Attributes["pass"].Value;
            var failed = status_node.Attributes["fail"].Value;

            Log(status_node.InnerText + " Passed=" + passed + " Failed=" + failed);
            // Pass Fail
            if (failed != "0")
            {
                // Something failed, email the administrater(s)

                var subject = "Alert " + SystemUnderTestName + " Operational Test failure";
                var body = "On " + DateTime.Now.ToString() + " The " + SystemUnderTestName +
                           " Operational Test reported a failure, see the report files attached.";
                SendEmailToAdmin(subject, body, SUT_AdminEmail, ProcessingFolder);
            }
            //  Archive results

            // create the archive sub folder
            var Archive_SubFolder = Path.Combine(ArchiveFolder, "Results_" + dt_Generated.Replace(" ", "").Replace(":", "").Replace(".", ""));
            for (var x = 0; x < 10; ++x)
            {
                if (!Directory.Exists(Archive_SubFolder)) break;
                Archive_SubFolder = Archive_SubFolder + x;
            }
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(Archive_SubFolder);
            }
            catch (Exception ex)
            {
                Log("Error: Could not create the Archive folder at " + Archive_SubFolder + Environment.NewLine +
                    ex.ToString());
                return;
            }
            // move files
            var files = Directory.GetFiles(ProcessingFolder);
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);
                var to = Path.Combine(Archive_SubFolder, filename);
                Log("Archive");
                Log(file);
                Log(to);
                File.Copy(file, to);
            }
            Console.ReadLine();

        }

        private static bool SendEmailToAdmin(string Subject, string Body, string toAddresses, string ProcessingFolder)
        {
            try
            {
                // <!-- Email Settings -->
                var Port = ConfigurationManager.AppSettings["Port"];
                var Host = ConfigurationManager.AppSettings["Host"];
                var EnableSsl = ConfigurationManager.AppSettings["EnableSsl"];
                var UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"];
                var LoginUserName = ConfigurationManager.AppSettings["LoginUserName"];
                var LoginPassword = ConfigurationManager.AppSettings["LoginPassword"];
                var FromAddress = ConfigurationManager.AppSettings["FromAddress"];

                var mail = new MailMessage { From = new MailAddress(FromAddress) };
                var adminemails = toAddresses.Split(';');
                if (!adminemails.Any())
                {
                    Log("Error, No admin emails to send failures to, " + toAddresses);
                    return false;
                }
                foreach (var emailaddress in adminemails)
                {
                    if (String.IsNullOrEmpty(emailaddress)) continue;
                    mail.To.Add(emailaddress);
                }

                mail.Subject = Subject;
                mail.IsBodyHtml = true;
                mail.Body = Body;
                mail.Priority = MailPriority.High;
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                var files = Directory.GetFiles(ProcessingFolder);
                foreach (var file in files)
                {
                    mail.Attachments.Add(new Attachment(file));
                }

                // setup Smtp Client
                var smtp = new SmtpClient
                {
                    Port = Convert.ToInt16(Port),
                    Host = Host,
                    EnableSsl = Convert.ToBoolean(EnableSsl),
                    UseDefaultCredentials = Convert.ToBoolean(UseDefaultCredentials),
                    Credentials = new NetworkCredential(LoginUserName, LoginPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Log("Error: Send email failed: " + Environment.NewLine + ex.ToString());
            }
            return false;
        }

        private static bool Log(string pLogEntry)
        {
            DateTime dt = DateTime.Now;
            Console.WriteLine("Log: " + dt.ToString("MMddyyyyhhmmss") + pLogEntry);
            if (!File.Exists(LogFile))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(LogFile))
                {
                    sw.WriteLine(pLogEntry);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(LogFile))
                {
                    sw.WriteLine(pLogEntry);
                }
            }
            return true;
        }

    }
}
