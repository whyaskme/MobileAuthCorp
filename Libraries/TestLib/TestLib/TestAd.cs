using MongoDB.Bson;

namespace TestLib
{
    public class TestAd
    {
        public TestAd()
        {
            _id = ObjectId.GenerateNewId();
            ClientId = ObjectId.Parse("000000000000000000000000");
            AID = "";
            Name = "";
            AdDesc = "";
            Catagory = "";
            TextAd = "";
            PageAd = "";
            Enabled = false;
        }
        public ObjectId _id { get; set; }
        public ObjectId ClientId { get; set; }
        public bool Enabled { get; set; }
        public string AID { get; set; }
        public string Name { get; set; }
        public string Catagory { get; set; }
        public string AdDesc { get; set; }
        public string TextAd { get; set; }
        public string PageAd { get; set; }
    }
}
