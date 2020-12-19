using System;
using System.Collections.Generic;
using MongoDB.Bson;
using ot = MACOperationalTestLib.OperationalTestConstants;

namespace MACOperationalTestLib
{
    /* operational test collection for the sybsystem */
    public class OperationalTest : OperationalTestUtils
    {
        public OperationalTest()
        { }
        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public string TestSystemName { get; set; }
        public string TestSystemIp { get; set; }
        public string TestSystemHost { get; set; }
        public int ResultsLookupDays { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime LastTimeTestRan { get; set; }
        public int RunInterval { get; set; }
        public OperationalTestEmailSettings EmailSettings { get; set; }
        public OperationalTestSmsSettings SmsSettings { get; set; }
        public List<OperationalTestSystemUnderTest> SystemsUnderTestList { get; set; }
    }
}