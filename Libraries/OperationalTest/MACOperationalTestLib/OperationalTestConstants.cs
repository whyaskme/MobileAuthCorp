using System;

namespace MACOperationalTestLib
{
    public class OperationalTestConstants
    {
        public const String TestApp = "pybot";
        public const String OperationalTestCollectionName = "OperationalTest";
        public const String OperationalTestResultsCollectionName = "OperationalTestResults";
        public const String resultsOutputFileName = "output.xml";
        public const String resultsReportFileName = "report.html";
        public const String resultsLogFileName = "log.html";

        public class Cfg
        {
            public const String TestSystemName = "TestSystemName";
            public const String ProcessingFolder = "ProcessingFolder";
            public const String ProcessingLogsFolder = "ProcessingLogFolder";
        }

        public class ReplacementKeys
        {
            public const String SUT = "[SUT]";
            public const String TESTSYSTEM = "[TESTSYSTEM]";
            public const String RESULTSSTATEMENT = "[RESULTSSTATEMENT]";
        }

        public class ServiceNames
        {
            public const String RequestOTP = "RequestOTP";
            public const String VerifyOTP = "VerifyOTP";
            public const String SecureTraidingRegister = "SecureTraidingRegister";
            public const String RegisteredUser = "RegisteredUser";
            public const String EndUserManagement = "EndUserManagement";
            public const String EndUserFileRegistration = "EndUserFileRegistration";
            public const String EndUserCompleteRegistration = "EndUserCompleteRegistration";
            public const String MessageBroadcastAPIService = "MessageBroadcastAPIService";
            public const String TwilioAPIService = "TwilioAPIService";
            public const String MacUsageBilling = "MacUsageBilling";
            public const String MacManageTypeDefsService = "MacManageTypeDefsService";
            public const String MacGroupInfo = "MacGroupInfo";
            public const String MacSystemStats = "MacSystemStats";
            public const String MacClientServices = "MacClientServices";
            public const String MacOpenEndUserServices = "MacOpenEndUserServices";
            public const String MacOpenClientServices = "MacOpenClientServices";
            public const String MacTestAdService = "MacTestAdService";
            public const String MACTestBank = "MACTestBank";
            public const String GetTestClientsInfo = "GetTestClientsInfo";
            public const String ClientServices = "ClientServices";
            public const String SecureAds = "SecureAds";
        }
    }
}
