using MongoDB.Bson;

namespace TestLib
{
    public class Bank
    {
        public Bank() { }

        public ObjectId _id { get; set; }
        public string BIN { get; set; }
        public int BillNumber { get; set; }
        public string Name { get; set; }

    }
}