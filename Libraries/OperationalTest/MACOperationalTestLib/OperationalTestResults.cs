using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MACOperationalTestLib
{
    /* contains the results of every operational test run by the subsystem */
    public class OperationalTestResults
    {
        public OperationalTestResults()
        {
            _id = ObjectId.GenerateNewId();
            _t = "OperationalTestResults";
            SystemUnderTestName = String.Empty;
            TestName = String.Empty;
            ScriptFile = String.Empty;
            ResultsStatement = String.Empty;
            ResultFileList = new List<OperationalTestResultFile>();
            RunDateTime = DateTime.UtcNow;
            EmailSent = false;
            TestRunTime = string.Empty;
            EmailDataTimeSent = string.Empty;
            EmailToAddresses = string.Empty;
            EmailSubject = string.Empty;
            EmailBody = string.Empty;

        }
        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public string SystemUnderTestName { get; set; }
        public string TestName { get; set; }
        public string ScriptFile { get; set; }
        public string ResultsStatement { get; set; }
        public DateTime RunDateTime { get; set; }
        public string TestRunTime { get; set; }
        public bool EmailSent { get; set; }
        public string EmailDataTimeSent { get; set; }
        public string EmailToAddresses { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public List<OperationalTestResultFile> ResultFileList { get; set; }
    }
}
