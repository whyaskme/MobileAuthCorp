using System;
using MongoDB.Bson;

namespace MACOperationalTestLib
{
    /* contains the configuration parameters for a collection of test run against a system */
    public class OperationalTestConfig
    {
        public OperationalTestConfig()
        {
            _id = ObjectId.GenerateNewId();
            _t = "OperationalTestConfig";
            DateLastUpdate = DateTime.UtcNow;
            eTestName = String.Empty;
            eTestScript = String.Empty; 
            eTestCommandLineVariables = String.Empty;
        }
        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public DateTime DateLastUpdate { get; set; }
        public string eTestName { get; set; }
        public string eTestScript { get; set; }
        public string eTestCommandLineVariables { get; set; }
    }
}
