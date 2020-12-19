using System.Collections.Generic;
using MongoDB.Bson;


namespace MACOperationalTestLib
{
    public class OperationalTestSystemUnderTest
    {
        public OperationalTestSystemUnderTest()
        {
            _id = ObjectId.GenerateNewId();
            _t = "OperationalTestSystemUnderTest";
            SystemName = SystemName;
            ContactList = new List<OperationalTestContact>();
            TestConfigList = new List<OperationalTestConfig>();
        }
        public ObjectId _id { get; set; }
        public string _t { get; set; }
        public string SystemName { get; set; }
        public List<OperationalTestContact> ContactList { get; set; }
        public List<OperationalTestConfig> TestConfigList { get; set; }
    }
}
